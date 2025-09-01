using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitFollow : UnitBehaviour
{
    [Header("Runtime")]
    [SerializeField] private Transform target;

    private NavMeshAgent navMeshAgent;

    public event UnityAction OnDestinationReached;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected override void OnBehaviourStart()
    {
        navMeshAgent.isStopped = false;
        Vector3 targetPosition = GetAdjacentPoint(target, 1f);

        navMeshAgent.SetDestination(targetPosition);

        StopAllCoroutines();
        StartCoroutine(FollowRoutine(target));
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        StopAllCoroutines();
        navMeshAgent.isStopped = true;
    }

    private IEnumerator FollowRoutine(Transform target)
    {
        yield return new WaitWhile(() => navMeshAgent.pathPending);

        var firstTime = true;

        while (true)
        {

            Vector3 destination = GetAdjacentPoint(target, 1f);
            navMeshAgent.SetDestination(destination);
            Unit.SetLookPosition(target.transform.position);
            yield return null;

            var destinationReached = Vector3.Distance(destination, transform.position) < 0.5f;

            if (destinationReached)
            {
                navMeshAgent.isStopped = true;
                if (firstTime)
                {
                    firstTime = false;
                    OnDestinationReached?.Invoke();
                }

            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }
    }

    private Vector3 GetAdjacentPoint(Transform target, float targetRadius, float maxDistance = 5f, int areaMask = NavMesh.AllAreas)
    {
        Vector3 agentPos = navMeshAgent.transform.position;
        Vector3 targetPos = target.position;

        // Direction from agent to target
        Vector3 dir = (targetPos - agentPos).normalized;

        // Offset so we stop at the edge of the target instead of its center
        Vector3 desiredPos = targetPos - dir * targetRadius;

        // Snap that offset point onto the NavMesh
        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, maxDistance, areaMask))
        {
            return hit.position;
        }

        // Fallback: just sample target position (so at least valid NavMesh)
        if (NavMesh.SamplePosition(targetPos, out hit, maxDistance, areaMask))
        {
            return hit.position;
        }

        return targetPos;
    }
}
