using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fungal material vs player material
public class UnitDance : UnitBehaviour
{
    [SerializeField] private ActivityReference danceReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Material targetMaterial; // assign in inspector
    [SerializeField] private string intensityID = "_Intensity";
    [SerializeField] private string danceAnimationClipName = "";
    [SerializeField] private float danceBeat = 1;
    [SerializeField] private float baseDanceBeat = 1;
    [SerializeField] private float moveDanceBeat = 1;

    [SerializeField] private Color targetColor;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color stageColor;

    [SerializeField] private float pulseSpeed = 10f;      // speed of pulse lerp
    [SerializeField] private float targetIntensity = 0f;      // speed of return to stage color
    [SerializeField] private float originalIntensity = 1f;
    [SerializeField] private float stageSpeed = 2f; // time to drop one stage if idle
    [SerializeField] private int maxProgress = 3;
    [SerializeField] private float pulseOffset = 1;
    [SerializeField] private int currentStage = 0;

    private Animator animator;
    private Material[] materials;

    private Coroutine animationSpeedRoutine;
    private Coroutine cheerRoutine;
    private Coroutine moveRoutine;
    private Coroutine highlightRoutine;

    private Vector3 originalPosition;

    private float BeatDuration => djReference.BeatDuration * danceBeat;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        animator = GetComponentInChildren<Animator>();

        // collect only the materials that are the same instance as targetMaterial
        var mats = new List<Material>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                if (!targetMaterial || mat.name.StartsWith(targetMaterial.name))
                {
                    mats.Add(mat);
                }
            }
        }

        materials = mats.ToArray();

        originalColor = Unit.Color;
    }

    protected override void OnBehaviourStart()
    {
        originalPosition = transform.position;

        currentStage = 0;

        animator.SetBool("IsDancing", true);

        danceBeat = baseDanceBeat;
        animationSpeedRoutine = StartCoroutine(UpdateAnimationSpeedLoop());
    }

    private IEnumerator UpdateAnimationSpeedLoop()
    {
        while (true)
        {
            if (!animator.IsInTransition(0))
            {
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo != null && clipInfo.Length > 0 && clipInfo[0].clip != null)
                {

                    float safeBeat = Mathf.Max(0.0001f, BeatDuration);
                    animator.speed = clipInfo[0].clip.length / safeBeat;
                }
            }

            yield return null; // check every frame
        }
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        animator.SetBool("IsDancing", false);
        StopCoroutine(animationSpeedRoutine);
    }

    public void IncrementDancePower()
    {
        animator.SetTrigger("Cheer");

        currentStage = Mathf.Clamp(currentStage + 1, 0, maxProgress);

        if (cheerRoutine != null) StopCoroutine(cheerRoutine);
        cheerRoutine = StartCoroutine(IncreaseDancePowerRoutine());
    }

    public void UseDanceMove(string animationName)
    {
        danceBeat = moveDanceBeat;

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(DanceMoveRoutine(animationName));
    }

    private IEnumerator DanceMoveRoutine(string animationName)
    {
        Unit.SetDestination(danceReference.Origin);
        yield return new WaitUntil(() => Unit.IsAtDestination);
        yield return new WaitForSeconds(1f);

        animator.ResetTrigger("Complete");
        animator.Play(animationName);

        yield return new WaitForSeconds(animator.speed * BeatDuration);

        animator.SetTrigger("Complete");
        danceBeat = baseDanceBeat;
        yield return new WaitForSeconds(1f);

        Unit.SetDestination(originalPosition);
        yield return new WaitUntil(() => Unit.IsAtDestination);
        animator.SetBool("IsDancing", true);
    }

    public void Highlight()
    {
        animator.SetTrigger("Cheer");

        if (highlightRoutine != null) StopCoroutine(highlightRoutine);
        highlightRoutine = StartCoroutine(HighlightRoutine());
    }

    public void Unhighlight()
    {
        if (highlightRoutine != null) StopCoroutine(highlightRoutine);
        highlightRoutine = StartCoroutine(UnhighlightRoutine());
    }

    private IEnumerator HighlightRoutine()
    {
        //var pulseValue = (float)(currentStage) / (maxProgress);
        var pulseValue = 1f;

        var pulseIntensity = Mathf.Lerp(originalIntensity, targetIntensity, pulseValue);

        var startColor = Unit.Color;
        var startIntensity = materials[0].GetFloat(intensityID);

        float t = 0f;
        while (t < 1f)
        {
            var pulseColor = Color.Lerp(originalColor, targetColor, pulseValue);

            t += Time.deltaTime * pulseSpeed;
            Unit.Color = Color.Lerp(startColor, pulseColor, t);
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat(intensityID, Mathf.Lerp(startIntensity, pulseIntensity, t));
            }
            yield return null;
        }
    }

    private IEnumerator UnhighlightRoutine()
    {
        var pulseValue = 0;

        var pulseIntensity = Mathf.Lerp(originalIntensity, targetIntensity, pulseValue);

        var startColor = Unit.Color;
        var startIntensity = materials[0].GetFloat(intensityID);

        float t = 0f;
        while (t < 1f)
        {
            var pulseColor = Color.Lerp(originalColor, targetColor, pulseValue);

            t += Time.deltaTime * pulseSpeed;
            Unit.Color = Color.Lerp(startColor, pulseColor, t);
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat(intensityID, Mathf.Lerp(startIntensity, pulseIntensity, t));
            }
            yield return null;
        }
    }

    private IEnumerator IncreaseDancePowerRoutine()
    {
        var pulseValue = (float)(currentStage) / (maxProgress);

        var pulseIntensity = Mathf.Lerp(originalIntensity, targetIntensity, pulseValue);

        var startColor = Unit.Color;
        var startIntensity = materials[0].GetFloat(intensityID);

        float t = 0f;
        while (t < 1f)
        {
            var pulseColor = Color.Lerp(originalColor, targetColor, pulseValue);

            t += Time.deltaTime * pulseSpeed;
            Unit.Color = Color.Lerp(startColor, pulseColor, t);
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat(intensityID, Mathf.Lerp(startIntensity, pulseIntensity, t));
            }
            yield return null;
        }

        //// Decrement stage timer
        while (currentStage > 0)
        {
            startColor = Unit.Color;
            startIntensity = materials[0].GetFloat(intensityID);

            t = 0f;
            while (t < 1f)
            {
                stageColor = Color.Lerp(originalColor, targetColor, (float)(currentStage - pulseOffset) / (maxProgress));

                t += Time.deltaTime * stageSpeed;
                Unit.Color = Color.Lerp(startColor, stageColor, t);
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat(intensityID, Mathf.Lerp(startIntensity, originalIntensity, t));
                }
                yield return null;
            }

            currentStage = Mathf.Clamp(currentStage - 1, 0, maxProgress);
            yield return null;
        }
    }

    private void DjReference_OnTrackValueChanged()
    {
        if (djReference.LeftTrack && djReference.RightTrack)
        {
            targetColor = Color.Lerp(djReference.LeftTrack.Glyph.Color, djReference.RightTrack.Glyph.Color, djReference.RightValue);
        }
        else targetColor = djReference.DominantTrack.Glyph.Color;
    }

    private void OnEnable()
    {
        djReference.OnTrackValueChanged += DjReference_OnTrackValueChanged;
    }

    private void OnDisable()
    {
        djReference.OnTrackValueChanged -= DjReference_OnTrackValueChanged;
    }
}
