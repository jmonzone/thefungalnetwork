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
        Initialize(0, 0, 100);
        originalScale = fillImage.transform.localScale;
    }

    public void Initialize(float value, float min, float max)
    {
        targetValue = value;
        slider.value = value;
        slider.minValue = min;
        slider.maxValue = max;
    }

    /// <summary>Externally call to update the bar target value.</summary>
    private void SetTargetValue(float value, bool pulse = true)
    {
        targetValue = value;

        if (pulse)
        {
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(PulseFill(pulseScale, originalScale));
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

    private IEnumerator PulseFill(float amplitude, Vector3 endScale)
    {
        if (!fillImage) yield break;

        float time = 0;

        float scale = 1;
        while (time < pulseDuration && Vector3.Distance(originalScale * scale, endScale) > 0.1f)
        {
            float progress = time / pulseDuration;
            scale = Mathf.Lerp(1f, amplitude, Mathf.Sin(progress * Mathf.PI));
            fillImage.transform.localScale = originalScale * scale;

            time += Time.deltaTime;
            yield return null;
        }

        fillImage.transform.localScale = endScale;
    }

    // Hold at max size (e.g. show "full power")
    public void SetTargetScale(float scale = 1.2f)
    {
        if (!fillImage) return;

        StopCoroutine(nameof(PulseFill));
        StartCoroutine(PulseFill(scale, originalScale * scale));
    }

    private Color ShiftColor(Color color, float shift)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = Mathf.Repeat(h + shift, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}
