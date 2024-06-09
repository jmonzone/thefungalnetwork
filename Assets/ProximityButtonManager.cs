using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProximityButtonManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> positionAnchors;

    private List<ActionButton> actionButtonList;

    public event UnityAction<EntityController> OnButtonClicked;

    private const float MAXIMUM_PROXIMITY_DISTANCE = 5f;

    private void Awake()
    {
        actionButtonList = GetComponentsInChildren<ActionButton>()
            .Select((button, index) =>
            {
                button.OnClicked += entity =>
                {
                    entity.UseAction();
                    OnButtonClicked?.Invoke(entity);
                };
                return button;
            })
            .ToList();
    }

    private void Update()
    {
        var closestEntities = GetClosestEntities();

        AssignEntitiesToButtons(closestEntities);
        AssignOrderToButtons();

    }

    private List<EntityController> GetClosestEntities()
    {
        var closestEntities = Physics.OverlapSphere(player.position, MAXIMUM_PROXIMITY_DISTANCE)
            .Select(collider => collider.GetComponentInParent<EntityController>())
            .Where(entity => entity != null && entity.HasInteraction)
            .OrderBy(entity => Vector3.Distance(player.position, entity.transform.position))
            .Distinct()
            .Take(actionButtonList.Count)
            .ToList();

        return closestEntities;
    }

    private void AssignEntitiesToButtons(List<EntityController> closestEntities)
    {
        var usedEntities = new HashSet<EntityController>();
        var usedButtons = new List<ActionButton>();

        foreach (var button in actionButtonList.Where(button => closestEntities.Contains(button.Entity)))
        {
            usedEntities.Add(button.Entity);
            usedButtons.Add(button);
        }

        foreach (var entity in closestEntities.Except(usedEntities))
        {
            var button = actionButtonList.FirstOrDefault(b => !usedButtons.Contains(b));
            if (button)
            {
                button.SetEntity(entity);
                usedEntities.Add(entity);
                usedButtons.Add(button);
            }
        }

        foreach (var button in actionButtonList.Where(button => !usedButtons.Contains(button)))
        {
            button.SetEntity(null);
        }
    }

    private void AssignOrderToButtons()
    {
        var buttonsWithEntities = actionButtonList.Where(button => button.Entity).ToList();
        buttonsWithEntities.Sort((button1, button2) => button1.Order.CompareTo(button2.Order));

        for (int i = 0; i < buttonsWithEntities.Count; i++)
        {
            var button = buttonsWithEntities[i];
            button.TargetPosition = positionAnchors[i].position;
            if (button.Order == 99) button.transform.parent.position = positionAnchors[i].position;
            button.Order = i;
        }
    }
}
