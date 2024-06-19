using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProximityButtonManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> positionAnchors;

    private List<ActionButton> actionButtonList;

    private const float MAXIMUM_PROXIMITY_DISTANCE = 3f;

    private void Awake()
    {
        actionButtonList = GetComponentsInChildren<ActionButton>().ToList();
    }

    private void Update()
    {
        var closestEntities = GetClosestEntities();

        AssignEntitiesToButtons(closestEntities);
        AssignOrderToButtons();

    }

    private List<ProximityAction> GetClosestEntities()
    {
        var closestEntities = Physics.OverlapSphere(player.position, MAXIMUM_PROXIMITY_DISTANCE)
            .Select(collider => collider.GetComponentInParent<ProximityAction>())
            .Where(action => action)
            .OrderBy(entity => Vector3.Distance(player.position, entity.transform.position))
            .Distinct()
            .Take(actionButtonList.Count)
            .ToList();

        return closestEntities;
    }

    private void AssignEntitiesToButtons(List<ProximityAction> closestEntities)
    {
        var usedEntities = new HashSet<ProximityAction>();
        var usedButtons = new List<ActionButton>();

        foreach (var button in actionButtonList.Where(button => closestEntities.Contains(button.ProximityAction)))
        {
            usedEntities.Add(button.ProximityAction);
            usedButtons.Add(button);
        }

        foreach (var action in closestEntities.Except(usedEntities))
        {
            var button = actionButtonList.FirstOrDefault(b => !usedButtons.Contains(b));
            if (button)
            {
                button.SetProximityAction(action);
                usedEntities.Add(action);
                usedButtons.Add(button);
            }
        }

        foreach (var button in actionButtonList.Where(button => !usedButtons.Contains(button)))
        {
            button.SetProximityAction(null);
        }
    }

    private void AssignOrderToButtons()
    {
        var buttonsWithEntities = actionButtonList.Where(button => button.ProximityAction).ToList();
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
