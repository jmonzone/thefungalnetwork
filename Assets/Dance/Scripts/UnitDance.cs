using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitDance : ActivityBehaviour
{
    [SerializeField] private ActivityReference danceReference;
    [SerializeField] private Skill danceSkill;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private string baseDanceAnimation;
    [SerializeField] private float danceBeat = 1;
    [SerializeField] private float baseDanceBeat = 1;
    [SerializeField] private float moveDanceBeat = 1;

    private Animator animator;
    private FresnelMaterialController materialController;

    private Coroutine moveRoutine;

    public event UnityAction<UnitDance, DanceMoveInstance> OnDanceMoveUsed;

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
        danceBeat = baseDanceBeat;
        StartCoroutine(DanceRoutine());

    }

    private IEnumerator DanceRoutine()
    {
        yield return new WaitUntil(() => Controller.IsAtDestination);
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
        animator.ResetTrigger("Complete");

        animator.SetBool("IsDancing", false);
        Controller.SetDestination(danceReference.Origin);
        yield return new WaitUntil(() => Controller.IsAtDestination);
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

        danceBeat = moveDanceBeat;

        // wait until we've completed `loops` iterations
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 + danceMove.Loops)
            yield return null;


        if (danceMove.Data.UseCompleteTrigger) animator.SetTrigger("Complete");
        else animator.Play(baseDanceAnimation, 0, 0);

        yield return new WaitForSeconds(1f);
        danceBeat = baseDanceBeat;

        Debug.Log("OnDanceMoveUsed");

        OnDanceMoveUsed?.Invoke(this, danceMove);
        animator.SetBool("IsDancing", false);
        Controller.SetDestination(ActivityPosition);

        yield return new WaitUntil(() => Controller.IsAtDestination);
        animator.SetBool("IsDancing", true);
        Debug.Log("OnDanceMoveComplete");

        yield return new WaitForSeconds(1f);

        Debug.Log("OnDanceMoveComplete");
        onComplete?.Invoke();
        isUsingDanceMove = false;
    }

    private void DjReference_OnTrackValueChanged()
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
        djReference.OnTrackValueChanged += DjReference_OnTrackValueChanged;
    }

    private void OnDisable()
    {
        djReference.OnTrackValueChanged -= DjReference_OnTrackValueChanged;
    }

    public override void OnSelect()
    {
        base.OnSelect();

        if (materialController) materialController.Highlight();
    }

    public override void OnUnselect()
    {
        base.OnUnselect();
        if (materialController) materialController.Unhighlight();
    }
}
