using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlyphButtonUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image glyphImage;
    [SerializeField] private DialogueGlyph glyph;
    [SerializeField] private Vector2 originalPosition;

    public DialogueGlyph Glyph => glyph;
    public Vector2 OriginalPosition => originalPosition;

    public event UnityAction<GlyphButtonUI> OnGlyphDropped;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void SetGlyph(DialogueGlyph glyph, Sprite sprite)
    {
        this.glyph = glyph;
        glyphImage.sprite = sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform); // move to top layer while dragging
        canvasGroup.blocksRaycasts = false;   // so it doesn’t block raycasts on drop targets
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Let drop zones handle placement
        canvasGroup.blocksRaycasts = true;
        OnGlyphDropped?.Invoke(this);

        if (transform.parent == canvas.transform) // means not dropped on a drop zone
        {
            ResetToOriginalParent();
        }
    }

    public void ResetToOriginalParent()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}
