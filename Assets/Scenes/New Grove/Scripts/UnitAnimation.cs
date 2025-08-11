using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        var unit = GetComponent<UnitController>();
        unit.OnSelected += Unit_OnSelected;

        var movement = GetComponent<UnitMovement>();
        movement.OnIsMovingHasChanged += Movement_OnIsMovingHasChanged;
    }

    private void Unit_OnSelected()
    {
        animator.Play("Jump");
    }

    private void Movement_OnIsMovingHasChanged(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }
}
