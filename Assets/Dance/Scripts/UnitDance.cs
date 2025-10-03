using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// fungal material vs player material
public class UnitDance : UnitBehaviour
{
    [SerializeField] private ActivityReference danceReference;
    [SerializeField] private Skill danceSkill;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Material targetMaterial; // assign in inspector
    [SerializeField] private string intensityID = "_Intensity";
    [SerializeField] private string baseDanceAnimation;
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

    private Coroutine cheerRoutine;
    private Coroutine moveRoutine;
    private Coroutine highlightRoutine;

    private Vector3 originalPosition;

    private float BeatDuration => djReference.BeatDuration * danceBeat;

    public event UnityAction<UnitController, DanceMoveInstance> OnDanceMoveUsed;
    public event UnityAction<UnitController, DanceMoveInstance> OnDanceMoveComplete;

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

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    protected override void OnBehaviourStart()
    {
        Debug.Log($"UnitDance.OnBehaviourStart");
        currentStage = 0;
        danceBeat = baseDanceBeat;

        StartCoroutine(DanceRoutine());
    }

    private IEnumerator DanceRoutine()
    {
        yield return new WaitUntil(() => Unit.IsAtDestination);
        animator.SetBool("IsDancing", true);
    }

    public override void StopBehaviour()
    {
        Debug.Log($"UnitDance.StopBehaviour");
        base.StopBehaviour();
        animator.SetBool("IsDancing", false);
        animator.speed = 1;
    }

    public void IncrementDancePower()
    {
        animator.SetTrigger("Cheer");

        currentStage = Mathf.Clamp(currentStage + 1, 0, maxProgress);

        if (cheerRoutine != null) StopCoroutine(cheerRoutine);
        cheerRoutine = StartCoroutine(IncreaseDancePowerRoutine());
    }

    public void UseDanceMove(DanceMoveInstance danceMove, UnityAction onComplete)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(DanceMoveRoutine(danceMove, onComplete));
    }

    private IEnumerator DanceMoveRoutine(DanceMoveInstance danceMove, UnityAction onComplete)
    {
        animator.ResetTrigger("Complete");

        animator.SetBool("IsDancing", false);
        Unit.SetDestination(danceReference.Origin);
        yield return new WaitUntil(() => Unit.IsAtDestination);
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

        OnDanceMoveUsed?.Invoke(Unit, danceMove);

        animator.SetBool("IsDancing", false);
        Unit.SetDestination(originalPosition);
        yield return new WaitUntil(() => Unit.IsAtDestination);
        animator.SetBool("IsDancing", true);

        yield return new WaitForSeconds(1f);

        onComplete?.Invoke();
        OnDanceMoveComplete?.Invoke(Unit, danceMove);
    }

    private bool isHighlighted = false;
    public void Highlight()
    {
        isHighlighted = true;
        //animator.SetTrigger("Cheer");

        if (highlightRoutine != null) StopCoroutine(highlightRoutine);
        highlightRoutine = StartCoroutine(HighlightRoutine());
    }

    public void Unhighlight()
    {
        isHighlighted = false;
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

        highlightRoutine = null;
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

        highlightRoutine = null;
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

            if (isHighlighted && highlightRoutine == null)
            {
                highlightRoutine = StartCoroutine(HighlightRoutine());
            }
        }
        else if (djReference.DominantTrack)
        {
            targetColor = djReference.DominantTrack.Glyph.Color;
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
}
