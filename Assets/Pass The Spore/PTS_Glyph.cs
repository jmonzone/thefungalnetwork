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
    [SerializeField] private float collectScale = 2f;
    [SerializeField] private float collectDuration = 0.75f;


    [Header("Float / Bounce Curve")]
    [SerializeField] private AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;

    private Vector3 targetLocalPosition;
    private bool isCollected = false;

    public float Duration => duration;

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

            image.color = Color.Lerp(Color.clear, Color.white, scaleT);

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
        Vector3 startScale = rectTransform.localScale;
        Color startColor = image.color;

        var targetScale = collectScale * Vector3.one;

        float elapsed = 0f;
        while (elapsed < collectDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / collectDuration);

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            image.color = Color.Lerp(startColor, Color.clear, t);

            // Optional upward drift
            rectTransform.localPosition += (1 - t) * 20f * Time.deltaTime * Vector3.up;

            yield return null;
        }

        Destroy(gameObject);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Collect();
    }
}