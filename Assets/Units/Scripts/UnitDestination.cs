using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitDestination : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private Vector3 destination;
    [SerializeField] private bool isAtDestination = true;

    public bool IsAtDestination => isAtDestination;

    private NavMeshAgent navMeshAgent;

    protected void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        navMeshAgent.isStopped = false;
        isAtDestination = false;
        navMeshAgent.SetDestination(destination);
    }

    private void Update()
    {
        if (!isAtDestination && Vector3.Distance(destination, transform.position) < 0.5f)
        {
            isAtDestination = true;
            //navMeshAgent.isStopped = true;
        }

    }
}
