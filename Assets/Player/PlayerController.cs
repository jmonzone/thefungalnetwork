using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitController
{
    [Header("Player References")]
    [SerializeField] private PlayerReference playerReference;

    private UnitFollow unitFollow;
    private UnitDestination unitDestination;

    protected override void Awake()
    {
        base.Awake();

        unitFollow = GetComponent<UnitFollow>();
        unitDestination = GetComponent<UnitDestination>();

        unitFollow.OnDestinationReached += UnitFollow_OnDestinationReached;

        playerReference.SetPlayer(this);
        playerReference.SetTargetPosition(transform.position);
    }

    private void UnitFollow_OnDestinationReached()
    {
        playerReference.TargetInteractable.Select();
    }

    private void OnEnable()
    {
        playerReference.OnTargetPositionChanged += PlayerReference_OnTargetPositionChanged;
        playerReference.OnTargetInteractableChanged += PlayerReference_OnTargetInteractableChanged; ;
    }

    private void OnDisable()
    {
        playerReference.OnTargetPositionChanged -= PlayerReference_OnTargetPositionChanged;
        playerReference.OnTargetInteractableChanged -= PlayerReference_OnTargetInteractableChanged;
    }

    private void PlayerReference_OnTargetInteractableChanged()
    {
        if (playerReference.TargetInteractable != null)
        {
            unitFollow.SetTarget(playerReference.TargetInteractable.Transform);
            SetBehaviour(unitFollow);
        }
    }

    private void PlayerReference_OnTargetPositionChanged()
    {
        if (playerReference.TargetInteractable == null)
        {
            SetLookPosition(playerReference.TargetPosition);
            unitDestination.SetDestination(playerReference.TargetPosition);
            SetBehaviour(unitDestination);
        }
    }
}