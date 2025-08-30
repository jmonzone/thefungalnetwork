using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitController
{
    [Header("Player References")]
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 2;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerReference.SetPlayer(this);
        playerReference.SetTargetPosition(transform.position);
    }

    private void Update()
    {
        var targetPosition = playerReference.TargetPosition;
        if (playerReference.TargetInteractable != null)
        {
            navMeshAgent.stoppingDistance = stoppingDistance;
            targetPosition = playerReference.TargetInteractable.Transform.position;
        }
        else
        {
            navMeshAgent.stoppingDistance = 0;
        }

        navMeshAgent.SetDestination(targetPosition);

        if (playerReference.TargetInteractable != null)
        {
            var destinationReached = navMeshAgent.remainingDistance < stoppingDistance;
            if (destinationReached && !navMeshAgent.pathPending) playerReference.InvokeOnDestinationReached();
        }

        LookAt(targetPosition);
    }

}