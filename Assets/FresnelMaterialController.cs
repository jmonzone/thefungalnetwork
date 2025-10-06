using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FresnelMaterialController : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;

    [SerializeField] private Color baseColor;
    [SerializeField] private Color highlightColor;

    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float highlightIntensity = 0f;

    [SerializeField] private float pulseDuration = 0.1f;      // speed of pulse lerp
    [SerializeField] private float pulseValue = 0.1f;      // speed of pulse lerp

    private float pulseTimer = 0;

    private Material[] materials;
    private bool isHighlighted = false;
    public bool IsHighlighted => isHighlighted;

    private Color CurrentColor => materials[0].GetColor("_Outer_Color");
    private float CurrentIntensity => materials[0].GetFloat("_Intensity");

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
        baseColor = CurrentColor;
        baseIntensity = CurrentIntensity;
    }

    public void SetTargetColor(Color color)
    {
        highlightColor = color;
    }

    public void Highlight()
    {
        isHighlighted = true;
        StopAllCoroutines();
        StartCoroutine(ColorRoutine(highlightColor));
        SetIntensity(highlightIntensity);
    }

    public void Unhighlight()
    {
        isHighlighted = false;
        StopAllCoroutines();
        StartCoroutine(ColorRoutine(baseColor));
        SetIntensity(baseIntensity);
    }

    public void Pulse()
    {
        pulseTimer += pulseValue;
        StopAllCoroutines();
        StartCoroutine(PulseRoutine());
    }

    private IEnumerator ColorRoutine(Color targetColor)
    {
        var startColor = CurrentColor;

        var t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            var lerpColor = Color.Lerp(startColor, targetColor, t / pulseDuration);
            SetColor(lerpColor);
            yield return null;
        }
    }

    private IEnumerator PulseRoutine()
    {

        var pulseColor = Color.Lerp(baseColor, highlightColor, pulseTimer);
        yield return ColorRoutine(pulseColor);

        while (pulseTimer > 0)
        {
            pulseTimer -= Time.deltaTime;

            var decayColor = Color.Lerp(baseColor, highlightColor, pulseTimer);

            SetColor(decayColor);
            SetIntensity(pulseTimer);
            yield return null;
        }
    }

    private void SetColor(Color targetColor)
    {
        foreach(var material in materials)
        {
            material.SetColor("_Outer_Color", targetColor);
        }
    }

    private void SetIntensity(float value)
    {
        foreach (var material in materials)
        {
            material.SetFloat("_Intensity", value);
        }
    }
}
