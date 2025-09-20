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
    [SerializeField] private RectTransform rectTransform;

    [Header("Settings")]
    [SerializeField] private float matchingDuration = 1f;

    [Header("Runtime")]
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
    }

    public void Initialize(GlyphData glyph, bool isPalleteGlyph, List<string> trappedWords)
    {
        this.glyph = glyph;
        this.isPalleteGlyph = isPalleteGlyph;
        glyphImage.sprite = glyph.Sprite;
        glyphImage.SetNativeSize();

        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform); // move to top while dragging
        canvasGroup.blocksRaycasts = false;    // don’t block raycasts
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        rectTransform.anchoredPosition = originalPosition;
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



}
