using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Transform renderRoot;

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
        if (playerReference.TargetUnit)
        {
            navMeshAgent.stoppingDistance = stoppingDistance;
            targetPosition = playerReference.TargetUnit.transform.position;
        }
        else
        {
            navMeshAgent.stoppingDistance = 0;
        }

        navMeshAgent.SetDestination(targetPosition);

        if (playerReference.TargetUnit)
        {
            var destinationReached = navMeshAgent.remainingDistance < stoppingDistance;
            if (destinationReached && !navMeshAgent.pathPending) playerReference.InvokeOnDestinationReached();
        }

        var lookDirection = targetPosition - transform.position;
        lookDirection.y = 0;

        if (lookDirection != Vector3.zero) renderRoot.forward = lookDirection;
    }

}