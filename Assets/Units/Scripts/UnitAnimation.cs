using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // Get current animation state info on layer 0
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.runtimeAnimatorController.animationClips.Any())
            return;

        // Find the clip currently playing
        AnimationClip currentClip = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(c => stateInfo.IsName(c.name));


        if (currentClip == null)
            return;

        float clipLength = currentClip.length;
        float secondsPerBeat = 60f / dJTableReference.BPM;
        float bpmMultiplier = clipLength / secondsPerBeat;


        animator.speed = bpmMultiplier;

        animator.Play(currentClip.name, 0, 0f);
        animator.Update(0f);
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void TriggerRespawn()
    {
        animator.SetTrigger("Respawn");
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
