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
    public event UnityAction<GlyphButtonUI, GlyphButtonUI> OnGlyphFused;
    // (draggedGlyph, targetGlyph)

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetGlyph(DialogueGlyph glyph, Sprite sprite)
    {
        this.glyph = glyph;
        glyphImage.sprite = sprite;

        rectTransform = GetComponent<RectTransform>();
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
            var otherGlyph = result.gameObject.GetComponent<GlyphButtonUI>();
            if (otherGlyph != null && otherGlyph != this)
            {
                Debug.Log("fuse");
                // 🔥 Trigger fusion
                OnGlyphFused?.Invoke(this, otherGlyph);
                return;
            }
        }

        if (transform.parent == canvas.transform) // means not dropped on a drop zone
        {
            ResetToOriginalParent();
        } 

        OnGlyphDropped?.Invoke(this);
    }

    public void ResetToOriginalParent()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}
