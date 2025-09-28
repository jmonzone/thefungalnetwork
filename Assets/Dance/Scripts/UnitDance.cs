using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDance : UnitBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Material targetMaterial; // assign in inspector
    [SerializeField] private string colorID = "_Outer_Color";
    [SerializeField] private string danceAnimationClipName = "";
    [SerializeField] private float danceSpeedModifier = 1;

    [SerializeField] private Color targetColor;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color stageColor;
    [SerializeField] private Color pulseColor;

    [SerializeField] private float pulseSpeed = 10f;      // speed of pulse lerp
    [SerializeField] private float targetIntensity = 0f;      // speed of return to stage color
    [SerializeField] private float originalIntensity = 1f;
    [SerializeField] private float stageSpeed = 2f; // time to drop one stage if idle
    [SerializeField] private int maxProgress = 3;
    [SerializeField] private float pulseOffset = 1;

    private Animator animator;
    private Material[] materialsToAnimate;

    [SerializeField] private int currentStage = 0;

    protected override void Awake()
    {
        base.Awake();
    }

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
                if (mat.shader == targetMaterial.shader && mat.name.StartsWith(targetMaterial.name))
                {
                    mats.Add(mat);
                }
            }
        }

        materialsToAnimate = mats.ToArray();
        originalColor = Unit.Color;
    }

    protected override void OnBehaviourStart()
    {
        targetColor = djReference.LeftTrack.Glyph.Color;
        currentStage = 0;

        animator.SetBool("IsDancing", true);

        // Replace with the exact state name your Animator uses.
        // Try "Base Layer.Dance" if the layer is "Base Layer", or just "Dance" if unique.
        StartCoroutine(SetAnimationSpeedWhenStateEntered(danceAnimationClipName, djReference.BeatDuration * danceSpeedModifier, 0));
    }

    private IEnumerator SetAnimationSpeedWhenStateEntered(string requiredStateName, float beatDuration, int layer = 0)
    {
        float elapsed = 0f;

        // Wait until the animator is *in the target state* and not in transition (or timeout)
        while (true)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            // IsName requires layerName.stateName or just stateName when unique.
            if (!animator.IsInTransition(layer) && stateInfo.IsName(requiredStateName))
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Safely get current clip info
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layer);
        if (clipInfo != null && clipInfo.Length > 0 && clipInfo[0].clip != null)
        {
            var clip = clipInfo[0].clip;
            SetAnimatorSpeedFromClip(clip, beatDuration);
            //Debug.Log($"Synced to clip '{clip.name}' (len {clip.length:F2}s) for beat {beatDuration:F2} {danceSpeedModifier:F2} s");
            yield break;
        }

        // Fallback: search controller clips for a name that matches the state
        var controller = animator.runtimeAnimatorController;
        if (controller != null)
        {
            AnimationClip fallback = null;
            foreach (var c in controller.animationClips)
            {
                if (c == null) continue;
                if (c.name.Equals(requiredStateName, StringComparison.OrdinalIgnoreCase) ||
                    c.name.IndexOf(requiredStateName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fallback = c;
                    break;
                }
            }

            if (fallback != null)
            {
                SetAnimatorSpeedFromClip(fallback, beatDuration);
                Debug.Log($"Fallback: synced to clip '{fallback.name}' (len {fallback.length:F2}s)");
                yield break;
            }
        }

        Debug.LogWarning("Could not find an animation clip to sync to. Consider using a StateMachineBehaviour on the target state to set animator.speed in OnStateEnter.");
    }

    private void SetAnimatorSpeedFromClip(AnimationClip clip, float beatDuration)
    {
        if (clip == null) return;

        //Debug.Log(clip.length);
        float safeBeat = Mathf.Max(0.0001f, beatDuration);
        // one loop of animation == one beat. Adjust if you want N beats per loop (e.g. clip.length / (beatDuration * beatsPerLoop))
        animator.speed = clip.length / safeBeat;
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        animator.SetBool("IsDancing", false);
    }

    public void IncrementDancePower()
    {
        animator.SetTrigger("Cheer");

        currentStage = Mathf.Clamp(currentStage + 1, 0, maxProgress);

        StopAllCoroutines();
        StartCoroutine(IncreaseRoutine());
    }

    private IEnumerator IncreaseRoutine()
    {
        var pulseValue = (float)(currentStage) / (maxProgress);

        pulseColor = Color.Lerp(originalColor, targetColor, pulseValue);
        var pulseIntensity = Mathf.Lerp(originalIntensity, targetIntensity, pulseValue);

        var currentIntensity = materialsToAnimate[0].GetFloat("_Intensity");

        var startColor = Unit.Color;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pulseSpeed;
            for (int i = 0; i < materialsToAnimate.Length; i++)
            {
                materialsToAnimate[i].SetColor(colorID, Color.Lerp(startColor, pulseColor, t));
                materialsToAnimate[i].SetFloat("_Intensity", Mathf.Lerp(currentIntensity, pulseIntensity, t));
            }
            yield return null;
        }

        // Decrement stage timer
        while (currentStage > 0)
        {
            stageColor = Color.Lerp(originalColor, targetColor, (float)(currentStage - pulseOffset) / (maxProgress));

            startColor = Unit.Color;
            currentIntensity = materialsToAnimate[0].GetFloat("_Intensity");

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * stageSpeed;
                for (int i = 0; i < materialsToAnimate.Length; i++)
                {
                    materialsToAnimate[i].SetColor(colorID, Color.Lerp(startColor, stageColor, t));
                    materialsToAnimate[i].SetFloat("_Intensity", Mathf.Lerp(currentIntensity, originalIntensity, t));
                }
                yield return null;
            }

            currentStage = Mathf.Clamp(currentStage - 1, 0, maxProgress);
            yield return null;
        }
    }
}
