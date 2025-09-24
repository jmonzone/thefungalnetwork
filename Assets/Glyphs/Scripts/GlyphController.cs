using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlyphController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Image glyphImage;
    [SerializeField] private Image background;
    [SerializeField] private RectTransform rectTransform;

    [Header("Settings")]
    [SerializeField] private float matchingDuration = 1f;

    [Header("Runtime")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private GlyphData glyph;
    [SerializeField] private bool isPalleteGlyph;
    [SerializeField] private Vector2 originalPosition;

    public GlyphData Glyph => glyph;
    public bool IsPalleteGlyph => isPalleteGlyph;
    public Vector2 OriginalPosition => originalPosition;
    public RectTransform RectTransform => rectTransform;
    public event UnityAction<GlyphController> OnGlyphDropped;
    public event UnityAction<GlyphController, GlyphController> OnGlyphFused;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        isInteractable = true;
    }

    public void Initialize(GlyphData glyph, bool isPalleteGlyph, List<string> trappedWords)
    {
        this.glyph = glyph;
        this.isPalleteGlyph = isPalleteGlyph;
        glyphImage.sprite = glyph.Sprite;
        glyphImage.SetNativeSize();

        originalPosition = rectTransform.position;
    }

    public void ToggleInteractable(bool value)
    {
        isInteractable = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;
        originalParent = transform.parent;
        transform.SetParent(canvas.transform); // move to top while dragging
        canvasGroup.blocksRaycasts = false;    // don’t block raycasts
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable || canvasGroup.blocksRaycasts) return;

        // Convert screen position of cursor into local position within the parent
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        rectTransform.anchoredPosition = localPoint;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractable || canvasGroup.blocksRaycasts) return;
        canvasGroup.blocksRaycasts = true;

        // Raycast to check if dropped onto another glyph
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var otherGlyph = result.gameObject.GetComponent<GlyphController>();
            if (otherGlyph != null && otherGlyph != this && !otherGlyph.isPalleteGlyph)
            {
                // 🔥 Trigger fusion
                OnGlyphFused?.Invoke(this, otherGlyph);
                return;
            }
        }

        if (transform.parent == canvas.transform) // means not dropped on a drop zone
        {
            ReturnToOriginalParent();
        }
        else
        {
            isPalleteGlyph = false;
        }

        OnGlyphDropped?.Invoke(this);
    }

    public void ReturnToOriginalParent()
    {
        transform.SetParent(originalParent);
        rectTransform.position = originalPosition;
    }


    public IEnumerator MoveAndSlot(RectTransform targetRect)
    {
        Vector3 startPos = RectTransform.position;
        Vector3 targetPos = targetRect.position;
        Vector3 targetScale = targetRect.localScale;

        float elapsed = 0f;
        float arcHeight = 50f;       // vertical arc for whimsical motion
        float wobbleStrength = 0.15f; // subtle wobble while moving
        float shrinkFactor = 0.5f;    // final shrink multiplier relative to target

        while (elapsed < matchingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / matchingDuration);
            float easeT = Mathf.SmoothStep(0f, 1f, t);

            // Arc movement
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
            Vector3 newWorldPos = Vector3.Lerp(startPos, targetPos, easeT) + new Vector3(0f, arc, 0f);

            // Wobble + scale shrinking toward target
            float scaleFactor = Mathf.Lerp(1f, shrinkFactor, easeT) * (1f + Mathf.Sin(t * Mathf.PI * 2f) * wobbleStrength);
            Vector3 newScale = targetScale * scaleFactor;

            RectTransform.position = newWorldPos;
            RectTransform.localScale = newScale;

            yield return null;
        }

        // Snap exactly to target and shrink completely
        RectTransform.position = targetPos;
        RectTransform.localScale = targetScale * shrinkFactor;

        RectTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// Separate outward from clashpoint, then spiral inward while shrinking + glowing.
    /// </summary>
    public IEnumerator AnimateSeparateAndSpiralIn(
        Vector3 clashPoint,
        float separateDuration = 0.3f,
        float spiralDuration = 0.7f,
        float startRadius = 100f,
        int spiralLoops = 2,
        float angleOffset = 0f)
    {
        Vector3 startScale = rectTransform.localScale;
        Color startColor = background.color;

        // --- Phase 1: Separate outward ---
        float elapsed = 0f;
        while (elapsed < separateDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / separateDuration;

            // Lerp radius from 0 → startRadius
            float radius = Mathf.Lerp(0f, startRadius, t);
            float angle = angleOffset;

            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            rectTransform.position = clashPoint + (Vector3)offset;

            rectTransform.localScale = Vector3.Lerp(startScale, startScale * 1.2f, t); // grow slightly
            background.color = Color.Lerp(startColor, Color.white, t);

            yield return null;
        }

        // --- Phase 2: Spiral inward ---
        elapsed = 0f;
        while (elapsed < spiralDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spiralDuration;

            // Spiral angle (same direction for both glyphs)
            float angle = Mathf.Lerp(0f, spiralLoops * Mathf.PI * 2f, t) + angleOffset;

            // Spiral radius shrinks from startRadius → 0
            float radius = Mathf.Lerp(startRadius, 0f, t);

            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            rectTransform.position = clashPoint + (Vector3)offset;

            rectTransform.localScale = Vector3.Lerp(startScale * 1.2f, Vector3.zero, t);
            background.color = Color.Lerp(startColor, Color.white, t); // stay white

            yield return null;
        }

        // Snap to clashpoint invisible
        rectTransform.position = clashPoint;
        rectTransform.localScale = Vector3.zero;
        background.color = startColor;
    }

    public IEnumerator AnimateElasticSpawn(
       float duration, float overshoot, float power, float frequency, float glowInt)
    {
        Vector3 baseScale = rectTransform.localScale;
        Color baseColor = background.color;

        rectTransform.localScale = Vector3.zero;
        background.color = Color.white;

        float elapsed = 0f;
        Vector3 lastScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Elastic ease out
            float elastic = Mathf.Sin(-frequency * (t + 1) * Mathf.PI / 2f) * Mathf.Pow(2f, -power * t) + 1f;
            float smoothT = Mathf.SmoothStep(0f, 1f, elastic);

            float scaleFactor = Mathf.Lerp(0f, overshoot, smoothT);
            lastScale = baseScale * scaleFactor;

            rectTransform.localScale = lastScale;

            // Glow intensity follows scale
            float glowT = Mathf.InverseLerp(overshoot, 1f, scaleFactor);
            background.color = Color.Lerp(Color.white, baseColor, glowT * glowInt);

            yield return null;
        }

        // Smoothly lerp final scale to baseScale
        float settleTime = 0.3f;
        elapsed = 0f;
        while (elapsed < settleTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settleTime;

            rectTransform.localScale = Vector3.Lerp(lastScale, baseScale, t);
            background.color = Color.Lerp(background.color, baseColor, t);

            yield return null;
        }

        rectTransform.localScale = baseScale;
        background.color = baseColor;
    }

    public IEnumerator AnimateFusionFail(
     Vector2 direction,
     float separateDistance,
     float separateDuration,
     float clashDistance,
     float clashDuration,
     float settleDuration,
     float scaleUpAmount,
     float wobbleStrength = 5f,
     Color? settleTint = null)
    {
        Vector3 startPos = rectTransform.anchoredPosition;
        Vector3 startScale = rectTransform.localScale;
        Color baseColor = background.color;
        Color targetTint = settleTint ?? baseColor * 0.9f;

        direction = direction.normalized;

        // --- Phase 1: Separate outward ---
        Vector3 separateTarget = startPos + (Vector3)(direction * separateDistance);
        float elapsed = 0f;
        while (elapsed < separateDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / separateDuration;

            rectTransform.anchoredPosition = Vector3.Lerp(startPos, separateTarget, t);
            rectTransform.localScale = Vector3.Lerp(startScale, startScale * (1f + scaleUpAmount * 0.5f), t);

            yield return null;
        }

        // --- Phase 2: Clash forward (scale up) ---
        elapsed = 0f;
        Vector3 clashTarget = separateTarget + (Vector3)(direction * clashDistance);
        while (elapsed < clashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / clashDuration);

            rectTransform.anchoredPosition = Vector3.Lerp(separateTarget, clashTarget, t);
            rectTransform.localScale = Vector3.Lerp(startScale * (1f + scaleUpAmount * 0.5f), startScale * (1f + scaleUpAmount), t);
            background.color = Color.Lerp(baseColor, Color.white * targetTint, t);

            yield return null;
        }

        // --- Phase 3: Settle back with subtle wobble and color lerp ---
        elapsed = 0f;
        Vector3 settleTarget = startPos + (Vector3)(direction * separateDistance * 0.8f);
        Color initialSettleColor = Color.white * targetTint;

        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / settleDuration);

            // Lerp position
            Vector3 basePos = Vector3.Lerp(clashTarget, settleTarget, t);

            // Add subtle perpendicular wobble
            Vector2 perp = new Vector2(-direction.y, direction.x);
            float wobble = Mathf.Sin(t * Mathf.PI * 4f) * wobbleStrength * (1 - t);
            rectTransform.anchoredPosition = basePos + (Vector3)(perp * wobble);

            // Lerp scale back to normal
            rectTransform.localScale = Vector3.Lerp(startScale * (1f + scaleUpAmount), startScale, t);

            // Smoothly lerp color back to base
            background.color = Color.Lerp(initialSettleColor, baseColor, t);

            yield return null;
        }

        // --- Final state ---
        rectTransform.anchoredPosition = settleTarget;
        rectTransform.localScale = startScale;
        background.color = baseColor; // fully back to original

    }
}
