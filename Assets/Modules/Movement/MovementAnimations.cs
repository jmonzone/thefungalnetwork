using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimations : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 1;

    public Animator Animator => animator;

    private void Awake()
    {
        if (!animator) enabled = false;
    }

    private void Update()
    {
        var isMoving = !movementController.IsAtDestination;
        animator.SetBool("isMoving", isMoving);
        animator.speed = animationSpeed;
        if (isMoving) animator.speed *= movementController.Direction.magnitude / 1.5f;
    }

    public void SetAnimatior(Animator animator)
    {
        this.animator = animator;
        enabled = animator;
    }
}
