using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollow : UnitBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 2;

    private NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void StartBehaviour()
    {
        var target = playerReference.Player.transform;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.SetDestination(target.position);

        StartCoroutine(FollowRoutine(target));
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        StopAllCoroutines();
    }

    private IEnumerator FollowRoutine(Transform target)
    {
        yield return new WaitWhile(() => navMeshAgent.pathPending);
        Debug.Log(navMeshAgent.isStopped);

        while (true)
        {
            var destination = target.transform.position;
            navMeshAgent.SetDestination(destination);
            Unit.LookAt(destination);
            yield return null;
        }
    }
}
