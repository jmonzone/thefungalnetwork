using System.Linq;
using UnityEngine;

public class ProximityInteraction : MonoBehaviour
{
    private const float MAXIMUM_PROXIMITY_DISTANCE = 3f;

    public ProximityAction TargeAction { get; private set; }

    private void Update()
    {
        var closestEntities = Physics.OverlapSphere(transform.position, MAXIMUM_PROXIMITY_DISTANCE)
           .Select(collider => collider.GetComponentInParent<ProximityAction>())
           .Where(action => action && action.transform != transform)
           .OrderBy(entity => Vector3.Distance(transform.position, entity.transform.position))
           .ToList();

        if (closestEntities.Count > 0) SetTargetAction(closestEntities.First());
        else SetTargetAction(null);
    }

    private void SetTargetAction(ProximityAction action)
    {
        if (TargeAction && TargeAction != action) TargeAction.SetInteractable(false);
        TargeAction = action;
        if (TargeAction) TargeAction.SetInteractable(true);
    }
}
