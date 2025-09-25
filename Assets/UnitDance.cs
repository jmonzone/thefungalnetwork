using System.Collections;
using UnityEngine;

public class UnitDance : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Material targetMaterial; // assign in inspector

    private Animator animator;
    private Material[] materialsToAnimate;
    private Color[] originalColors;

    private float danceTimer = 0;
    [SerializeField] private float danceDuration = 1f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        // collect only the materials that are the same instance as targetMaterial
        var mats = new System.Collections.Generic.List<Material>();

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

        // store original colors
        originalColors = new Color[materialsToAnimate.Length];
        for (int i = 0; i < materialsToAnimate.Length; i++)
        {
            originalColors[i] = materialsToAnimate[i].GetColor("_Outer_Color");
        }

        originalColor = originalColors[0];
    }

    [SerializeField] private Color targetColor;
    [SerializeField] private Color originalColor;

    [SerializeField] private float pulseSpeed = 10f;      // speed of pulse lerp
    [SerializeField] private float returnSpeed = 5f;      // speed of return to stage color
    [SerializeField] private float maxProgress = 0.9f;    // max progression fraction
    [SerializeField] private float stageDecayInterval = 2f; // time to drop one stage if idle

    private int currentStage = 0;
    private float stageTimer = 0f;

    private void Update()
    {
        // Decrement stage timer
        if (currentStage > 0)
        {
            stageTimer -= Time.deltaTime;
            if (stageTimer <= 0f)
            {
                currentStage--;
                UpdateAnimator();
                stageTimer = stageDecayInterval;
                StartCoroutine(StageColorUpdate());
            }
        }
    }

    public void StartDance()
    {
        targetColor = djReference.LeftTrack.Glyph.Color;

        currentStage = Mathf.Clamp(currentStage + 1, 0, 3);
        stageTimer = stageDecayInterval; // reset timer on click
        UpdateAnimator();

        StopCoroutine(nameof(PulseRoutine));
        StartCoroutine(PulseRoutine());
    }

    private void UpdateAnimator()
    {
        if (currentStage <= 1)
            animator.SetBool("IsDancing", false);
        else
            animator.SetBool("IsDancing", true);
    }

    private IEnumerator PulseRoutine()
    {
        // Determine current stage color
        float progressFraction = currentStage / 3f;
        Color stageColor = Color.Lerp(originalColor, targetColor, Mathf.Min(progressFraction, maxProgress));
        Color pulseColor = Color.Lerp(stageColor, targetColor, 0.2f); // slightly brighter

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pulseSpeed;
            for (int i = 0; i < materialsToAnimate.Length; i++)
            {
                Color current = materialsToAnimate[i].GetColor("_Outer_Color");
                materialsToAnimate[i].SetColor("_Outer_Color", Color.Lerp(current, pulseColor, t));
            }
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * returnSpeed;
            for (int i = 0; i < materialsToAnimate.Length; i++)
            {
                Color current = materialsToAnimate[i].GetColor("_Outer_Color");
                materialsToAnimate[i].SetColor("_Outer_Color", Color.Lerp(current, stageColor, t));
            }
            yield return null;
        }
    }

    // Optional: smoothly update color when stage decrements automatically
    private IEnumerator StageColorUpdate()
    {
        float progressFraction = currentStage / 3f;
        Color stageColor = Color.Lerp(originalColor, targetColor, Mathf.Min(progressFraction, maxProgress));

        bool done = false;
        while (!done)
        {
            done = true;
            for (int i = 0; i < materialsToAnimate.Length; i++)
            {
                Color current = materialsToAnimate[i].GetColor("_Outer_Color");
                Color next = Color.Lerp(current, stageColor, Time.deltaTime * returnSpeed);
                materialsToAnimate[i].SetColor("_Outer_Color", next);

                if (((Vector4)(next - stageColor)).sqrMagnitude > 0.0001f)
                    done = false;
            }
            yield return null;
        }
    }



}
