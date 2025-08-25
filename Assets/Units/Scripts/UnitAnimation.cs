using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    [SerializeField] private DJTableReference dJTableReference;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        var unit = GetComponent<UnitController>();
        unit.OnSelected += Unit_OnSelected;

        //var movement = GetComponent<UnitMovement>();
        //movement.OnIsMovingHasChanged += Movement_OnIsMovingHasChanged;

        var unitAI = GetComponent<UnitAI>();
        unitAI.OnIsMovingHasChanged += Movement_OnIsMovingHasChanged;
    }

    private void OnEnable()
    {
        dJTableReference.OnBPMChanged += DJTableReference_OnBPMChanged;
    }

    private void OnDisable()
    {
        dJTableReference.OnBPMChanged -= DJTableReference_OnBPMChanged;
    }

    private void DJTableReference_OnBPMChanged()
    {
        AnimationClip clip = animator.runtimeAnimatorController.animationClips[0]; // the clip to sync

        float clipLength = clip.length;
        float secondsPerBeat = 60f / dJTableReference.BPM;
        float bpmMultiplier = clipLength / secondsPerBeat;

        animator.speed = bpmMultiplier;
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
