using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProximityInteraction : MonoBehaviour
{
    [SerializeField] private Button interactionButton;
    [SerializeField] private PlayerController playerController;

    private const float MAXIMUM_PROXIMITY_DISTANCE = 3f;

    private ProximityAction targetAction;

    private void Awake()
    {
        interactionButton.onClick.AddListener(() => targetAction.Use());

        enabled = playerController.Movement;
    }

    private void Update()
    {
        if (!playerController.Movement) return;
        interactionButton.interactable = TryFindProximityInteraction();
    }

    private bool TryFindProximityInteraction()
    {

        var closestEntities = Physics.OverlapSphere(playerController.Movement.transform.position, MAXIMUM_PROXIMITY_DISTANCE)
            .Select(collider => collider.GetComponentInParent<ProximityAction>())
            .Where(action => action && action.transform != playerController.Movement.transform)
            .OrderBy(entity => Vector3.Distance(playerController.Movement.transform.position, entity.transform.position))
            .ToList();

        if (closestEntities.Count > 0) SetTargetAction(closestEntities.First());
        else SetTargetAction(null);

        return targetAction;
    }

    private void SetTargetAction(ProximityAction action)
    {
        if (targetAction && targetAction != action) targetAction.SetInteractable(false);
        targetAction = action;
        if (targetAction) targetAction.SetInteractable(true);
    }


}
