using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorBackgroundOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private DJTableReference dJTableReference;

    [Header("Overlay Settings")]
    [SerializeField] private RawImage overlayImage;           // overlay above render textures
    [SerializeField] private Gradient flashGradient;          // gradient for colors
    [SerializeField] private float[] flashDurations = { 0.05f, 0.1f, 0.2f }; // flash durations
    [SerializeField] private float[] alphaValues = { 0.05f, 0.1f, 0.2f };    // alpha for each flash

    private bool overlayActive = false;
    private int lastDurationIndex = -1;
    private Color targetColor;

    private void OnEnable()
    {
        dancefloorReference.OnDancefloorEnter += OnDancefloorEnter;
        dancefloorReference.OnDancefloorExit += OnDancefloorExit;
    }

    private void OnDisable()
    {
        dancefloorReference.OnDancefloorEnter -= OnDancefloorEnter;
        dancefloorReference.OnDancefloorExit -= OnDancefloorExit;

        if (dJTableReference != null)
            dJTableReference.OnBeat -= OnBeatStep;
    }

    private void OnDancefloorEnter()
    {
        if (dJTableReference != null)
            dJTableReference.OnBeat += OnBeatStep;

        overlayActive = true;
    }

    private void OnDancefloorExit()
    {
        if (dJTableReference != null)
            dJTableReference.OnBeat -= OnBeatStep;

        overlayActive = false;
        ResetOverlay();
        StopAllCoroutines();
    }

    private void OnBeatStep(int step)
    {
        if (!overlayActive || overlayImage == null) return;

        StopAllCoroutines();
        StartCoroutine(ApplyNextPattern());
    }

    private IEnumerator ApplyNextPattern()
    {
        if (overlayImage == null) yield break;

        // pick a random color from gradient
        Color startColor = overlayImage.color; // current color

        // pick a random flash duration (avoid immediate repeat)
        int index;
        do
        {
            index = Random.Range(0, flashDurations.Length);
        } while (flashDurations.Length > 1 && index == lastDurationIndex);

        lastDurationIndex = index;
        float duration = flashDurations[index];
        float alpha = alphaValues[index]; // constant alpha

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / duration);
            Color lerped = Color.Lerp(startColor, targetColor, blend);
            overlayImage.color = new Color(lerped.r, lerped.g, lerped.b, alpha);
            yield return null;
        }

        // hold at the target color until next beat/pattern
        overlayImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
    }

    private void ResetOverlay()
    {
        if (overlayImage != null)
            overlayImage.color = new Color(1f, 1f, 1f, 0f);
    }

    /// <summary>Externally call this to pick a new flash manually.</summary>
    public void PickNextPattern()
    {
        if (!overlayActive) return;

        // pick a random color from gradient
        targetColor = flashGradient.Evaluate(Random.Range(0f, 1f));

        StopAllCoroutines();
        StartCoroutine(ApplyNextPattern());
    }
}
