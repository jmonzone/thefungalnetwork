using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimations : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 1;

    private void Update()
    {
        animator.SetBool("isMoving", !movementController.IsAtDestination);
        animator.speed = animationSpeed * movementController.Direction.magnitude / 1.5f;
    }
}
