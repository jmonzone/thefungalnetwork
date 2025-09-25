using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorEnergyController : MonoBehaviour
{
    [Header("References")]
    public DancefloorAuraController auraController;
    public RectTransform particleParent;
    public GameObject energyParticlePrefab;
    public Image touchIndicator;

    [Header("Particle Settings")]
    public float jumpHeight = 80f;
    public float spawnScaleDuration = 0.1f;
    public float landingScaleDuration = 0.1f;
    public float spawnScaleAmount = 1.2f;
    public float landingScaleAmount = 1.2f;

    [Header("Touch Indicator")]
    public float touchDuration = 0.2f;
    public float touchScale = 1.5f;
    public Color touchColor = Color.white;

    private Coroutine touchRoutine;
    private bool isActive = true;
    private float beatDuation;

    // ---------------------------
    // Public interface for manager
    // ---------------------------
    public void Activate(float beatDuation)
    {
        isActive = true;
        this.beatDuation = beatDuation;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void SendEnergy(Vector3 screenPosition)
    {
        SpawnEnergyParticle(screenPosition, beatDuation);
        ShowTouchIndicator(Input.mousePosition);
    }

    // ---------------------------
    // Particle spawning logic
    // ---------------------------
    private void SpawnEnergyParticle(Vector3 screenPosition, float travelDuration)
    {
        if (!energyParticlePrefab) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            particleParent,
            screenPosition,
            null,
            out Vector2 localPos
        );

        GameObject particle = Instantiate(energyParticlePrefab, particleParent);
        RectTransform particleRect = particle.GetComponent<RectTransform>();
        particleRect.anchoredPosition = localPos;
        particleRect.localScale = Vector3.zero;

        Vector2 auraPos = auraController.rect.anchoredPosition;
        StartCoroutine(MoveParticle(particleRect, auraPos, travelDuration));
    }

    private IEnumerator MoveParticle(RectTransform particle, Vector2 targetPos, float travelDuration)
    {
        CanvasGroup cg = particle.GetComponent<CanvasGroup>();
        if (cg == null) cg = particle.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        Vector2 startPos = particle.anchoredPosition;
        Vector2 arcPeak = Vector2.Lerp(startPos, targetPos, 0.5f) + Vector2.up * jumpHeight;

        float spawnElapsed = 0f;
        while (spawnElapsed < spawnScaleDuration)
        {
            spawnElapsed += Time.deltaTime;
            float t = spawnElapsed / spawnScaleDuration;
            particle.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * spawnScaleAmount, t);
            yield return null;
        }

        float xOffsetAmplitude = Random.Range(20f, 80f);
        float elapsed = 0f;

        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelDuration);

            Vector2 pos = Mathf.Pow(1 - t, 2) * startPos +
                          2 * (1 - t) * t * arcPeak +
                          Mathf.Pow(t, 2) * targetPos;
            //pos.x += Mathf.Sin(t * Mathf.PI * 2f) * xOffsetAmplitude * (1f - t);

            cg.alpha = t;

            particle.anchoredPosition = pos;
            particle.localScale = Vector3.Lerp(Vector3.one * spawnScaleAmount, Vector3.one, t);
            yield return null;
        }

        float landingElapsed = 0f;
       

        while (landingElapsed < landingScaleDuration)
        {
            landingElapsed += Time.deltaTime;
            float t = landingElapsed / landingScaleDuration;
            particle.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            cg.alpha = 1f - t;
            yield return null;
        }

        Destroy(particle.gameObject);
        auraController.Hit();
    }

    private void ShowTouchIndicator(Vector3 screenPos)
    {
        if (!touchIndicator || !isActive) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchIndicator.transform.parent as RectTransform,
            screenPos,
            null,
            out Vector2 localPos
        );

        touchIndicator.rectTransform.anchoredPosition = localPos;

        if (touchRoutine != null) StopCoroutine(touchRoutine);
        touchRoutine = StartCoroutine(TouchIndicatorRoutine());
    }

    private IEnumerator TouchIndicatorRoutine()
    {
        touchIndicator.gameObject.SetActive(true);
        touchIndicator.transform.localScale = Vector3.zero;
        touchIndicator.color = touchColor;

        float elapsed = 0f;
        while (elapsed < touchDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / touchDuration;
            touchIndicator.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * touchScale, t);
            touchIndicator.color = new Color(touchColor.r, touchColor.g, touchColor.b, 1f - t);
            yield return null;
        }

        touchIndicator.gameObject.SetActive(false);
    }
}
