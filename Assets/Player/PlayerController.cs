using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitController
{
    [Header("Player References")]
    [SerializeField] private PlayerReference playerReference;

    private NavMeshAgent navMeshAgent;
    private UnitFollow unitFollow;
    private bool isAtDestination;

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();
        unitFollow = GetComponent<UnitFollow>();

        unitFollow.OnDestinationReached += UnitFollow_OnDestinationReached;

        playerReference.SetPlayer(this);
        playerReference.SetTargetPosition(transform.position);
    }

    private void UnitFollow_OnDestinationReached()
    {
        playerReference.TargetInteractable.OnSelect();
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
            navMeshAgent.isStopped = false;
            unitFollow.SetTarget(playerReference.TargetInteractable.Transform);
            unitFollow.StartBehaviour();
        }
    }

    private void PlayerReference_OnTargetPositionChanged()
    {
        unitFollow.StopBehaviour();

        isAtDestination = false;
        navMeshAgent.isStopped = false;
        var targetPosition = playerReference.TargetPosition;
        navMeshAgent.SetDestination(targetPosition);
    }

    protected override void Update()
    {
        base.Update();

        if (playerReference.TargetInteractable == null)
        {
            LookAt(playerReference.TargetPosition);

            if (!isAtDestination && Vector3.Distance(playerReference.TargetPosition, transform.position) < 0.5f)
            {
                isAtDestination = true;
                navMeshAgent.isStopped = true;
            }
        }

    }
}