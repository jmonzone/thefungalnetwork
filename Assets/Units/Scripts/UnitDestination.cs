using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitDestination : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private Vector3 destination;
    [SerializeField] private Transform target;
    [SerializeField] private bool isAtDestination = true;

    public bool IsAtDestination => isAtDestination;

    private NavMeshAgent navMeshAgent;

    protected void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination)
    {
        Debug.Log("setting destination");

        target = null;

        this.destination = destination;
        navMeshAgent.isStopped = false;
        isAtDestination = false;
        navMeshAgent.SetDestination(destination);
    }

    public void SetTarget(Transform target)
    {
        Debug.Log("setting target");

        this.target = target;
        navMeshAgent.isStopped = false;
        isAtDestination = false;
    }

    private void Update()
    {
        if (target)
        {
            Debug.Log("updating targett");
            destination = target.position;
            navMeshAgent.SetDestination(destination);
        }

        if (!isAtDestination && Vector3.Distance(destination, transform.position) < 0.5f)
        {

            isAtDestination = true;
        }

    }
}
