using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitDJ : UnitBehaviour
{
    [SerializeField] private DJTableReference djReference;

    private NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void OnBehaviourStart()
    {
        navMeshAgent.enabled = false;
        transform.position = djReference.DjTable.DJPosition;

    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        navMeshAgent.enabled = true;
    }
}
