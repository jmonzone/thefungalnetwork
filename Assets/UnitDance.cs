using System.Collections;
using UnityEngine;

public class UnitDance : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Material targetMaterial; // assign in inspector

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
    private Color[] originalColors;

    [SerializeField] private int currentStage = 0;

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

    public void StartDance()
    {
        targetColor = djReference.LeftTrack.Glyph.Color;
        currentStage = 0;

        animator.SetBool("IsDancing", true);

    }

    public void EndDance()
    {
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

        var currentColor = materialsToAnimate[0].GetColor("_Outer_Color");
        var currentIntensity = materialsToAnimate[0].GetFloat("_Intensity");

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pulseSpeed;
            for (int i = 0; i < materialsToAnimate.Length; i++)
            {
                materialsToAnimate[i].SetColor("_Outer_Color", Color.Lerp(currentColor, pulseColor, t));
                materialsToAnimate[i].SetFloat("_Intensity", Mathf.Lerp(currentIntensity, pulseIntensity, t));
            }
            yield return null;
        }

        // Decrement stage timer
        while (currentStage > 0)
        {
            stageColor = Color.Lerp(originalColor, targetColor, (float)(currentStage - pulseOffset) / (maxProgress));

            currentColor = materialsToAnimate[0].GetColor("_Outer_Color");
            currentIntensity = materialsToAnimate[0].GetFloat("_Intensity");

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * stageSpeed;
                for (int i = 0; i < materialsToAnimate.Length; i++)
                {
                    materialsToAnimate[i].SetColor("_Outer_Color", Color.Lerp(currentColor, stageColor, t));
                    materialsToAnimate[i].SetFloat("_Intensity", Mathf.Lerp(currentIntensity, originalIntensity, t));
                }
                yield return null;
            }

            currentStage = Mathf.Clamp(currentStage - 1, 0, maxProgress);
            yield return null;
        }
    }

}
