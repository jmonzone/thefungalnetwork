using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitWander : UnitBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 4f;
    [SerializeField] private float gravitateStrength = 0.5f;

    [Header("Runtime")]
    [SerializeField] private bool isIdle;

    public bool IsIdle => isIdle;

    private NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void OnBehaviourStart()
    {
        StartWander();
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        PauseBehaviour();
    }

    public override void PauseBehaviour()
    {
        base.PauseBehaviour();
        StopWander();
    }

    public override void UnpauseBehaviour()
    {
        base.UnpauseBehaviour();
        StartWander();
    }

    private void StartWander()
    {
        navMeshAgent.stoppingDistance = 0.1f;
        navMeshAgent.isStopped = false;
        StartCoroutine(WanderRoutine());
    }

    private void StopWander()
    {
        navMeshAgent.isStopped = true;
        StopAllCoroutines();
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            var targetPosition = GetReachableRandomDestination(transform.position, wanderRadius, NavMesh.AllAreas);
            navMeshAgent.SetDestination(targetPosition);
            navMeshAgent.speed = baseSpeed * Random.Range(0.8f, 1.2f);

            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                Controller.SetLookPosition(targetPosition);
                yield return null;
            }

            var idleTargetTime = Random.Range(minIdleTime, maxIdleTime);
            isIdle = true;
            navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(idleTargetTime);
            isIdle = false;
            navMeshAgent.isStopped = false;
        }
    }

    private Vector3 GetReachableRandomDestination(Vector3 origin, float radius, int layermask, int maxAttempts = 10)
    {
        // assume center of the NavMesh is Vector3.zero (or origin of surface)
        Vector3 navMeshCenter = Vector3.back * 4f;

        for (int i = 0; i < maxAttempts; i++)
        {
            // 1. Pick a random direction + radius (nonlinear to favor inner area)
            Vector3 randomDirection = Random.onUnitSphere;
            float biasedRadius = Mathf.Pow(Random.value, 2f) * radius; // square favors inner points
            Vector3 randomPoint = origin + randomDirection * biasedRadius;

            // 3. Apply gravitation (optional extra pull)
            randomPoint = Vector3.Lerp(randomPoint, navMeshCenter, gravitateStrength);

            // 4. Project onto NavMesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, layermask))
            {
                // 5. Validate path
                NavMeshPath path = new NavMeshPath();
                if (navMeshAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    return hit.position;
                }
            }
        }

        // Fallback
        return origin;
    }

}
