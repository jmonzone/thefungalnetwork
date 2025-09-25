using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DancefloorAuraController : MonoBehaviour
{
    [Header("References")]
    public Image auraCircle;
    public RectTransform rect;

    [Header("Aura Settings")]
    public float lerpSpeed = 8f;
    public float scaleAmount = 1.4f;
    public Color normalColor = Color.gray;
    public Color auraColor = Color.green; // single target color
    public float pulseDuration = 0.15f;

    [Header("BPM Settings")]
    public float beatInterval = 1f;

    [Header("Rhythm Settings")]
    public int beatsToFullAura = 3;
    public float maxTapGap = 2f;
    public float decayPerBeat = 0.1f;

    [Header("Ring Pulse Settings")]
    public GameObject ringPrefab;
    public float ringDuration = 0.4f;
    public float ringMaxScale = 2f;

    private float auraProgress = 0f;
    private float auraScale = 1f;
    private float hitPulse = 0f;
    private bool isActive = false;

    private float lastTapTime = -1f;
    private int consecutiveHits = 0;

    public event UnityAction OnHit;

    // ---------------------------
    // Public interface for manager
    // ---------------------------
    public void Activate() => isActive = true;
    public void Deactivate() => isActive = false;

    public void ResetAura()
    {
        auraProgress = 0f;
        auraScale = 1f;
        hitPulse = 0f;
        lastTapTime = -1f;
        consecutiveHits = 0;
        auraCircle.color = normalColor * 0.5f;
        auraCircle.transform.localScale = Vector3.one;
    }

    public void Hit()
    {
        if (!isActive) return;

        float currentTime = Time.time;
        float delta = lastTapTime > 0f ? currentTime - lastTapTime : beatInterval;
        lastTapTime = currentTime;

        // increment consecutive hits if within allowed gap
        if (delta <= maxTapGap)
            consecutiveHits++;
        else
            consecutiveHits = 1;

        auraProgress = Mathf.Clamp((float)consecutiveHits / beatsToFullAura, 0f, 1f);

        hitPulse = 1f;

        // Spawn ring pulse on hit
        SpawnRingPulse();

        OnHit?.Invoke();
    }

    private void Update()
    {
        if (!isActive) return;
        UpdateAura();
    }

    private void UpdateAura()
    {
        // decay auraProgress over time
        float decayThisFrame = (decayPerBeat / beatInterval) * Time.deltaTime;
        auraProgress -= decayThisFrame;
        auraProgress = Mathf.Clamp(auraProgress, 0f, 1f);

        // Apply hit pulse: lerp toward auraColor, then back to current progress
        Color targetColor = Color.Lerp(normalColor, auraColor, auraProgress);
        if (hitPulse > 0f)
        {
            targetColor = Color.Lerp(targetColor, auraColor, hitPulse);
            hitPulse -= Time.deltaTime / pulseDuration;
        }

        // Smooth color & alpha
        Color currentColor = auraCircle.color;
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * lerpSpeed);
        currentColor.a = Mathf.Lerp(currentColor.a, 0.5f + 0.5f * auraProgress, Time.deltaTime * lerpSpeed);
        auraCircle.color = currentColor;

        // Smooth scale
        float pulseScale = 1f + (scaleAmount - 1f) * auraProgress + hitPulse * 0.2f;
        auraScale = Mathf.Lerp(auraScale, pulseScale, Time.deltaTime * lerpSpeed);
        auraCircle.transform.localScale = Vector3.one * auraScale;
    }

    private void SpawnRingPulse()
    {
        if (!ringPrefab) return;

        GameObject ring = Instantiate(ringPrefab, rect);
        RectTransform ringRect = ring.GetComponent<RectTransform>();
        ringRect.localScale = Vector3.one;

        StartCoroutine(AnimateRing(ringRect));
    }

    private System.Collections.IEnumerator AnimateRing(RectTransform ring)
    {
        float elapsed = 0f;
        CanvasGroup cg = ring.GetComponent<CanvasGroup>();
        if (cg == null) cg = ring.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        while (elapsed < ringDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ringDuration;

            ring.localScale = Vector3.one * Mathf.Lerp(1f, ringMaxScale, t);
            cg.alpha = 1f - t;

            yield return null;
        }

        Destroy(ring.gameObject);
    }
}
