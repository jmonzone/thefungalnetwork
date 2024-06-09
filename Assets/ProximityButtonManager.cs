using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProximityButtonManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    private List<ActionButton> actionButtonList;

    public event UnityAction<EntityController> OnButtonClicked;

    private const float MAXIMUM_PROXIMITY_DISTANCE = 4f;

    private void Awake()
    {
        actionButtonList = GetComponentsInChildren<ActionButton>().ToList();

        foreach(var button in actionButtonList)
        {
            button.OnClicked += entity =>
            {
                entity.UseAction();
                OnButtonClicked?.Invoke(entity);
            };
        }
        
    }

    private void Update()
    {
        var closestEntities = GetClosestEntities();

        AssignEntitiesToButtons(closestEntities);
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

    //public void SetProximityAction(EntityController entity)
    //{
    //    if (currentState != UIState.JOYSTICK) return;
    //    if (proximityEntity == entity) return;

    //    proximityEntity = entity;

    //    if (entity)
    //    {
    //        actionButton.SetInteraction(entity);

    //        if (entity is FungalController fungal)
    //        {
    //            this.fungal = fungal;

    //            feedPanel.Fungal = fungal.Model;
    //            UpdateEscortButtonText();
    //        }
    //    }

    //    actionButton.SetVisible(entity);

    //}
}
