using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PartyVibeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage; // assign your fill image here
    [SerializeField] private Gradient vibeGradient; // base gradient driven by slider value

    [Header("Lerp Settings")]
    [SerializeField] private float lerpSpeed = 5f;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseDuration = 0.3f;
    [SerializeField] private float pulseScale = 1.2f;

    [Header("Color Animation")]
    [SerializeField] private float colorPulseSpeed = 2f; // how fast the hue shifts
    [SerializeField] private float colorPulseStrength = 0.15f; // how strong the shift is

    private Coroutine pulseRoutine;
    private float targetValue;

    private void Awake()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        targetValue = slider.value;
    }

    private void OnEnable()
    {
        partyReference.OnScoreChanged += PartyReference_OnScoreChanged;
    }

    private void OnDisable()
    {
        partyReference.OnScoreChanged -= PartyReference_OnScoreChanged;
    }

    private void Update()
    {
        // Smooth lerp to target value
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * lerpSpeed);

        // Base gradient color
        float t = slider.normalizedValue;
        Color baseColor = vibeGradient.Evaluate(t);

        // Add animated color vibe (oscillates the hue/brightness)
        float wave = Mathf.Sin(Time.time * colorPulseSpeed) * colorPulseStrength;
        Color animated = ShiftColor(baseColor, wave);

        fillImage.color = animated;
    }

    private void PartyReference_OnScoreChanged()
    {
        targetValue = partyReference.Score;

        // Pulse effect
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseFill());
    }

    private IEnumerator PulseFill()
    {
        Vector3 originalScale = fillImage.transform.localScale;

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

    // Shifts color by modifying HSV values slightly
    private Color ShiftColor(Color color, float shift)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = Mathf.Repeat(h + shift, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}
