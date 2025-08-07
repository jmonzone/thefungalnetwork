using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        var movement = GetComponent<CharacterMovement>();
        movement.OnIsMovingHasChanged += Movement_OnIsMovingHasChanged;
    }

    private void Movement_OnIsMovingHasChanged(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }
}
