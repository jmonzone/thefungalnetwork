using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PTS_Glyph : MonoBehaviour, IPointerEnterHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.2f;

    [Header("Float / Bounce Curve")]
    [SerializeField] private AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    // Designers can edit this curve in inspector for arcs, bounces, etc.

    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;

    private Vector3 targetLocalPosition;
    private bool isCollected = false;

    public UnityAction<PTS_Glyph> OnCollected;

    public void InitializeAtPosition(Vector3 localPosition, RectTransform container)
    {
        rectTransform.SetParent(container, false);
        rectTransform.localPosition = localPosition;
        targetLocalPosition = localPosition;

        float initialScale = minScale;
        rectTransform.localScale = Vector3.one * initialScale;

        StartCoroutine(AnimateGlyph(initialScale));
    }

    private IEnumerator AnimateGlyph(float initialScale)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Scale animation
            float scaleT = floatCurve.Evaluate(t);
            float scale = Mathf.Lerp(initialScale, maxScale, scaleT);
            rectTransform.localScale = Vector3.one * scale;

            // Vertical floating/bounce controlled by curve
            float yOffset = floatCurve.Evaluate(t) * 20f; // 20 units max, or scale as needed
            rectTransform.localPosition = targetLocalPosition + new Vector3(0f, yOffset, 0f);

            // Fade out
            if (image != null)
            {
                Color c = image.color;
                c.a = Mathf.Lerp(0f, 1f, scaleT);
                image.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        OnCollected?.Invoke(this);
        StopAllCoroutines();
        StartCoroutine(LerpOutAndDestroy());
    }

    private IEnumerator LerpOutAndDestroy()
    {
        float elapsed = 0f;
        float fadeDuration = 0.4f;
        Vector3 startScale = rectTransform.localScale;
        Color startColor = image.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            rectTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            if (image != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, t);
                image.color = c;
            }

            // Optional upward drift
            rectTransform.localPosition += Vector3.up * Time.deltaTime * 20f * (1 - t);

            yield return null;
        }

        Destroy(gameObject);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Collect();
    }
}