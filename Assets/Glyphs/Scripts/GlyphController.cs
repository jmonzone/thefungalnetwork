using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlyphController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Image glyphImage;
    [SerializeField] private RectTransform rectTransform;

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

    public void Initialize(GlyphData glyph, bool isPalleteGlyph)
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
        var results = new System.Collections.Generic.List<RaycastResult>();
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
}
