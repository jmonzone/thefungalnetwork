using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAnimation : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        var isMoving = navMeshAgent.velocity.magnitude > 0.0001f;
        animator.SetBool("IsMoving", isMoving);

        animator.transform.localPosition = Vector3.zero;
    }
}
