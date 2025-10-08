using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DanceActivityUnit : ActivityBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private string baseDanceAnimation;

    private Animator animator;
    private FresnelMaterialController materialController;

    private Coroutine moveRoutine;

    public event UnityAction<DanceActivityUnit, DanceMoveInstance> OnDanceMoveUsed;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        animator = GetComponentInChildren<Animator>();

        materialController = GetComponent<FresnelMaterialController>();
        if (materialController) materialController.Initialize();
    }

    protected override void OnBehaviourStart()
    {
        //Debug.Log($"UnitDance.OnBehaviourStart");
        StartCoroutine(DanceRoutine());

    }

    private IEnumerator DanceRoutine()
    {
        yield return new WaitUntil(() => Controller.Destination.IsAtDestination);
        animator.SetBool("IsDancing", true);
    }

    public override void StopBehaviour()
    {
        //Debug.Log($"UnitDance.StopBehaviour");
        base.StopBehaviour();
        animator.SetBool("IsDancing", false);
        animator.speed = 1;
    }

    public void IncrementDancePower()
    {
        animator.SetTrigger("Cheer");
        //materialController.Pulse();
    }

    private bool isUsingDanceMove = false;
    public bool IsUsingDanceMove => isUsingDanceMove;

    public void UseDanceMove(DanceMoveInstance danceMove, UnityAction onComplete)
    {
        isUsingDanceMove = true;
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(DanceMoveRoutine(danceMove, onComplete));
    }

    private IEnumerator DanceMoveRoutine(DanceMoveInstance danceMove, UnityAction onComplete)
    {
        Debug.Log("OnDanceMoveStarted");

        animator.ResetTrigger("Complete");

        animator.SetBool("IsDancing", false);
        Controller.Destination.SetDestination(Activity.Origin);
        yield return new WaitUntil(() => Controller.Destination.IsAtDestination);
        animator.SetBool("IsDancing", true);

        yield return new WaitForSeconds(1f);

        var animationName = danceMove.Data.AnimationName;

        // Play the spin
        animator.Play(animationName, 0, 0);

        // let the animator update so state info becomes valid
        yield return null;

        // wait until the animator is actually in the spin state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            yield return null;


        // wait until we've completed `loops` iterations
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 + danceMove.Loops)
            yield return null;


        if (danceMove.Data.UseCompleteTrigger) animator.SetTrigger("Complete");
        else animator.Play(baseDanceAnimation, 0, 0);

        yield return new WaitForSeconds(1f);

        Debug.Log("OnDanceMoveUsed");

        OnDanceMoveUsed?.Invoke(this, danceMove);
        animator.SetBool("IsDancing", false);
        Controller.Destination.SetDestination(ActivityPosition);

        yield return new WaitUntil(() => Controller.Destination.IsAtDestination);
        animator.SetBool("IsDancing", true);

        yield return new WaitForSeconds(1f);

        Debug.Log("OnDanceMoveComplete");
        onComplete?.Invoke();
        isUsingDanceMove = false;
    }

    private void UpdateTargetColor()
    {
        if (!materialController) return;

        if (djReference.LeftTrack && djReference.RightTrack)
        {
            var blendedColor = Color.Lerp(djReference.LeftTrack.Glyph.Color, djReference.RightTrack.Glyph.Color, djReference.RightValue);
            materialController.SetTargetColor(blendedColor);

        }
        else if (djReference.DominantTrack)
        {
            var targetColor = djReference.DominantTrack.Glyph.Color;
            materialController.SetTargetColor(targetColor);
        }
    }

    private void OnEnable()
    {
        djReference.OnTrackValueChanged += UpdateTargetColor;
    }

    private void OnDisable()
    {
        djReference.OnTrackValueChanged -= UpdateTargetColor;
    }

    public override void OnSelect()
    {
        base.OnSelect();

        if (materialController)
        {
            UpdateTargetColor();
            materialController.Highlight();
        }
    }

    public override void OnUnselect()
    {
        base.OnUnselect();
        if (materialController) materialController.Unhighlight();
    }
}
