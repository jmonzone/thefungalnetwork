using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ValueBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient gradient; // slider color mapping

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float pulseDuration = 0.3f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float colorPulseSpeed = 2f;
    [SerializeField] private float colorPulseStrength = 0.15f;

    [Header("Runtime")]
    [SerializeField] private float targetValue;
    [SerializeField] private Color animatedColor;

    private Coroutine pulseRoutine;
    private Vector3 originalScale;

    public Color AnimatedColor => animatedColor;

    private void Awake()
    {
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = 100;
            targetValue = slider.value;
        }

        originalScale = fillImage.transform.localScale;
    }

    /// <summary>Externally call to update the bar target value.</summary>
    public void SetTargetValue(float value, bool pulse = true)
    {
        targetValue = value;

        if (pulse)
        {
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(PulseFill());
        }
    }

    public void Increment() => SetTargetValue(targetValue + 1);

    private void Update()
    {
        if (!slider) return;

        // Smooth lerp
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * lerpSpeed);

        // Base color from gradient
        float t = slider.normalizedValue;
        Color baseColor = gradient.Evaluate(t);

        // Animate hue
        float wave = Mathf.Sin(Time.time * colorPulseSpeed) * colorPulseStrength;
        animatedColor = ShiftColor(baseColor, wave);

        if (fillImage) fillImage.color = animatedColor;
    }

    private IEnumerator PulseFill()
    {
        if (!fillImage) yield break;

        float time = 0;

        while (time < pulseDuration)
        {
            float progress = time / pulseDuration;
            float scale = Mathf.Lerp(1f, pulseScale, Mathf.Sin(progress * Mathf.PI));
            fillImage.transform.localScale = originalScale * scale;

            time += Time.deltaTime;
            yield return null;
        }

        fillImage.transform.localScale = originalScale;
    }

    private Color ShiftColor(Color color, float shift)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = Mathf.Repeat(h + shift, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}
