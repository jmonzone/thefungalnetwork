using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitDestination : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private Vector3 destination;
    [SerializeField] private Transform target;
    [SerializeField] private bool isAtDestination = true;

    public Vector3 Destination => destination;
    public Transform Target => target;
    public bool IsAtDestination => isAtDestination;

    private NavMeshAgent navMeshAgent;

    public event UnityAction OnTargetSelected;

    protected void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;

    }

    public void SetDestination(Vector3 destination)
    {
        //Debug.Log("Setting destination");
        target = null;
        this.destination = destination;
        navMeshAgent.isStopped = false;
        isAtDestination = false;
        navMeshAgent.SetDestination(destination);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        navMeshAgent.isStopped = false;
        isAtDestination = false;

        OnTargetSelected?.Invoke();
    }

    private void Update()
    {
        if (target)
        {
            // Direction from self to target
            Vector3 direction = (target.position - transform.position).normalized;

            // Desired stopping position 1 unit away
            Vector3 offsetDestination = target.position - direction * 1f;

            destination = offsetDestination;
            navMeshAgent.SetDestination(destination);
        }

        if (!isAtDestination && Vector3.Distance(destination, transform.position) < 0.5f)
        {
            isAtDestination = true;
        }
    }
}
