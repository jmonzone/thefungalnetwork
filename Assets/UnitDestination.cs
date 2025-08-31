using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitDestination : UnitBehaviour
{
    [Header("Runtime")]
    [SerializeField] private Vector3 destination;
    [SerializeField] private bool isAtDestination;

    public bool IsAtDestination => isAtDestination;

    private NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    public override void StartBehaviour()
    {
        isAtDestination = false;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
        StartCoroutine(DestinationRoutine());
    }

    private IEnumerator DestinationRoutine()
    {
        while(Vector3.Distance(destination, transform.position) > 0.5f)
        {
            yield return null;
        }

        isAtDestination = true;
        navMeshAgent.isStopped = true;
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        navMeshAgent.isStopped = false;
    }
}
