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

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float pulseDuration = 0.3f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float colorPulseSpeed = 2f; // how fast the hue shifts
    [SerializeField] private float colorPulseStrength = 0.15f; // how strong the shift is

    [Header("Runtime")]
    [SerializeField] private float targetValue;
    [SerializeField] private Color animatedColor;

    private Coroutine pulseRoutine;
    private PartyVibeParticleController partyVibeParticleController;

    public Color AnimatedColor => animatedColor;

    private void Awake()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        targetValue = slider.value;

        partyVibeParticleController = GetComponent<PartyVibeParticleController>();
        partyVibeParticleController.OnParticlesReached += PartyVibeParticleController_OnParticlesReached;
    }

    private void PartyVibeParticleController_OnParticlesReached()
    {
        targetValue = partyReference.Score;

        // Pulse effect
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseFill());
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
        animatedColor = ShiftColor(baseColor, wave);

        fillImage.color = animatedColor;
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
