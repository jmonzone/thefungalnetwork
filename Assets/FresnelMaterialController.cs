using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FresnelMaterialController : MonoBehaviour
{
   // speed of return to stage color

    [SerializeField] private float pulseOffset = 1;
    [SerializeField] private float pulseDuration = 0.1f;      // speed of pulse lerp
    [SerializeField] private float stageDuration = 0.1f;      // speed of pulse lerp

    [SerializeField] private int currentStage = 0;
    [SerializeField] private int maxProgress = 3;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;
    [SerializeField] private Color stageColor;

    [SerializeField] private Material targetMaterial;
    [SerializeField] private float originalIntensity = 1f;
    [SerializeField] private float startIntensity;
    [SerializeField] private float targetIntensity = 0f;

    private Material[] materials;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
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
    }


    public void SetTargetColor(Color color)
    {
        targetColor = color;
    }

    public void Pulse()
    {
        currentStage = Mathf.Clamp(currentStage + 1, 0, maxProgress);
        startColor = CurrentColor;
        startIntensity = CurrentIntensity;
        StopAllCoroutines();
        StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        var pulseValue = (float)(currentStage) / (maxProgress);
        var pulseIntensity = Mathf.Lerp(originalIntensity, targetIntensity, pulseValue);

        float t = 0f;
        while (t < pulseDuration)
        {
            var pulseColor = Color.Lerp(originalColor, targetColor, pulseValue);

            t += Time.deltaTime;
            SetColor(pulseColor, pulseIntensity, t / pulseDuration);
            yield return null;
        }

        //// Decrement stage timer
        while (currentStage > 0)
        {
            startColor = CurrentColor;
            startIntensity = CurrentIntensity;

            t = 0f;
            while (t < stageDuration)
            {
                stageColor = Color.Lerp(originalColor, targetColor, (float)(currentStage - pulseOffset) / (maxProgress));

                t += Time.deltaTime;

                SetColor(stageColor, originalIntensity, t / stageDuration);

                yield return null;
            }

            currentStage = Mathf.Clamp(currentStage - 1, 0, maxProgress);
            yield return null;
        }
    }

    private Color CurrentColor => materials[0].GetColor("_Outer_Color");
    private float CurrentIntensity => materials[0].GetFloat("_Intensity");

    private void SetColor(Color targetColor, float targetIntensity, float t)
    {
        foreach(var material in materials)
        {
            material.SetColor("_Outer_Color", Color.Lerp(startColor, targetColor, t));
            material.SetFloat("_Intensity", Mathf.Lerp(startIntensity, targetIntensity, t));

        }
    }
}
