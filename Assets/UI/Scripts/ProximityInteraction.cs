using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProximityInteraction : MonoBehaviour
{
    [SerializeField] private Button interactionButton;
    [SerializeField] private Transform proximityHolder;

    private const float MAXIMUM_PROXIMITY_DISTANCE = 3f;

    private void Update()
    {
        interactionButton.interactable = TryFindProximityInteraction();
    }

    private bool TryFindProximityInteraction()
    {
        var closestEntities = Physics.OverlapSphere(proximityHolder.position, MAXIMUM_PROXIMITY_DISTANCE)
            .Select(collider => collider.GetComponentInParent<ProximityAction>())
            .Where(action => action)
            .OrderBy(entity => Vector3.Distance(proximityHolder.position, entity.transform.position))
            .ToList();

        return closestEntities.Count > 0;
    }


}
