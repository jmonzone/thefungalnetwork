using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MusicVideoPostProcessing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorReference musicVideoReference;
    [SerializeField] private DJTableReference dJTableReference;
    [SerializeField] private Volume volume;

    [Header("Settings")]
    [SerializeField, Tooltip("Bloom intensity when a beat hits.")]
    private float bloomIntensityOnBeat = 2f;

    [SerializeField, Tooltip("Chromatic aberration intensity when a beat hits.")]
    private float chromaIntensityOnBeat = 0.5f;

    [SerializeField, Tooltip("Bloom threshold during beat pulse (0-1).")]
    private float bloomThresholdOnBeat = 0.3f;

    [SerializeField, Tooltip("Base pulse duration in seconds (can be scaled by beat).")]
    private float pulseDuration = 0.1f;

    [SerializeField, Tooltip("Scale pulse duration relative to beat duration.")]
    private bool scalePulseByBeat = true;

    [Header("Pattern Settings")]
    [SerializeField, Tooltip("Possible bloom intensities per fade step.")]
    private float[] bloomPatterns = new float[] { 1f, 2f, 3f };

    [SerializeField, Tooltip("Possible chroma intensities per fade step.")]
    private float[] chromaPatterns = new float[] { 0.2f, 0.5f, 0.8f };

    [SerializeField, Tooltip("Possible bloom thresholds per fade step.")]
    private float[] bloomThresholdPatterns = new float[] { 0.1f, 0.3f, 0.5f };


    private Bloom bloom;
    private ChromaticAberration chromatic;

    private float defaultBloomIntensity;
    private float defaultChromaIntensity;
    private float defaultBloomThreshold;

    private void Awake()
    {
        if (volume == null)
            volume = FindObjectOfType<Volume>();

        if (volume != null && volume.profile != null)
        {
            if (!volume.profile.TryGet(out bloom))
                Debug.LogWarning("Bloom not found in Volume profile!");
            if (!volume.profile.TryGet(out chromatic))
                Debug.LogWarning("Chromatic Aberration not found in Volume profile!");

            if (bloom != null)
            {
                defaultBloomIntensity = bloom.intensity.value;
                defaultBloomThreshold = bloom.threshold.value;
            }

            if (chromatic != null)
                defaultChromaIntensity = chromatic.intensity.value;
        }
    }

    private void OnEnable()
    {
        musicVideoReference.OnDancefloorStart += MusicVideoReference_OnMusicVideoStart;
        musicVideoReference.OnDancefloorExit += MusicVideoReference_OnMusicVideoEnd;
    }

    private void OnDisable()
    {
        musicVideoReference.OnDancefloorStart -= MusicVideoReference_OnMusicVideoStart;
        musicVideoReference.OnDancefloorExit -= MusicVideoReference_OnMusicVideoEnd;

        if (dJTableReference != null)
            dJTableReference.OnBeat -= OnBeatStep;
    }

    private void MusicVideoReference_OnMusicVideoStart()
    {
        if (dJTableReference != null)
            dJTableReference.OnBeat += OnBeatStep;
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        if (dJTableReference != null)
            dJTableReference.OnBeat -= OnBeatStep;

        StopAllCoroutines();
        ResetEffects();
    }

    private void ResetEffects()
    {
        if (bloom != null)
        {
            bloom.intensity.value = defaultBloomIntensity;
            bloom.threshold.value = defaultBloomThreshold;
        }

        if (chromatic != null)
            chromatic.intensity.value = defaultChromaIntensity;
    }

    public void OnBeatStep(int step)
    {
        if (bloom == null || chromatic == null) return;

        StopAllCoroutines();

        float duration = scalePulseByBeat && dJTableReference != null
            ? dJTableReference.BeatDuration * pulseDuration
            : pulseDuration;

        StartCoroutine(PulseEffects(duration));
    }

    private IEnumerator PulseEffects(float duration)
    {
        float t = 0f;

        float startBloom = bloom.intensity.value;
        float startThreshold = bloom.threshold.value;
        float startChroma = chromatic.intensity.value;

        // Fade in
        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / duration);

            bloom.intensity.value = Mathf.Lerp(startBloom, bloomIntensityOnBeat, blend);
            bloom.threshold.value = Mathf.Lerp(startThreshold, bloomThresholdOnBeat, blend);
            chromatic.intensity.value = Mathf.Lerp(startChroma, chromaIntensityOnBeat, blend);

            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / duration);

            bloom.intensity.value = Mathf.Lerp(bloomIntensityOnBeat, startBloom, blend);
            bloom.threshold.value = Mathf.Lerp(bloomThresholdOnBeat, startThreshold, blend);
            chromatic.intensity.value = Mathf.Lerp(chromaIntensityOnBeat, startChroma, blend);

            yield return null;
        }
    }

    public void PickNextPattern()
    {
        bloomIntensityOnBeat = bloomPatterns[Random.Range(0, bloomPatterns.Length)];
        chromaIntensityOnBeat = chromaPatterns[Random.Range(0, chromaPatterns.Length)];
        bloomThresholdOnBeat = bloomThresholdPatterns[Random.Range(0, bloomThresholdPatterns.Length)];
    }

}
