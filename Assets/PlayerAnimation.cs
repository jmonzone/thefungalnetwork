using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimation : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        var isMoving = navMeshAgent.velocity.magnitude > 0.0001f;
        animator.SetBool("isMoving", isMoving);

        animator.transform.localPosition = Vector3.zero;
    }
}
