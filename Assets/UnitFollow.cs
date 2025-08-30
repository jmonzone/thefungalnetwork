using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollow : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform renderRoot;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 2;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void StartFollow(Transform target)
    {
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.SetDestination(target.transform.position);

        StartCoroutine(FollowRoutine(target));
    }

    private IEnumerator FollowRoutine(Transform target)
    {
        yield return new WaitWhile(() => navMeshAgent.pathPending);
        Debug.Log(navMeshAgent.isStopped);

        while (true)
        {
            navMeshAgent.SetDestination(target.transform.position);

            //var lookDirection = target.transform.position - transform.position;
            //lookDirection.y = 0;

            //if (lookDirection != Vector3.zero) renderRoot.forward = lookDirection;

            yield return null;
        }

    }
}
