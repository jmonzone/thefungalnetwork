using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlyphController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Image backgroundGlyph;
    [SerializeField] private Image glyphImage;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GlyphTextController textPrefab;

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
    private List<GlyphTextController> textControllers = new List<GlyphTextController>();

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(GlyphData glyph, bool isPalleteGlyph, List<string> trappedWords)
    {
        this.glyph = glyph;
        this.isPalleteGlyph = isPalleteGlyph;
        backgroundGlyph.enabled = isPalleteGlyph;
        glyphImage.sprite = glyph.Sprite;
        glyphImage.SetNativeSize();

        originalPosition = rectTransform.anchoredPosition;

        SpawnTrappedWords(trappedWords);
    }

    private void SpawnTrappedWords(List<string> words)
    {
        foreach (var word in words)
        {
            var textController = Instantiate(textPrefab, rectTransform);
            textController.Initialize(word);
            textController.transform.SetAsFirstSibling();
            textController.transform.localPosition = Vector3.zero; // start at center of glyph
            textController.transform.localScale = Vector3.one;

            textControllers.Add(textController);
        }
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

    public IEnumerator Animate(Vector3 startCanvasPos, Vector3 targetCanvasPos, float duration, float scalePulse)
    {
        RectTransform.localPosition = startCanvasPos;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            RectTransform.localPosition = Vector3.Lerp(
                startCanvasPos,
                targetCanvasPos,
                Mathf.SmoothStep(0, 1, normalized)
            );

            RectTransform.localScale = Vector3.one * (1f + scalePulse * Mathf.Sin(normalized * Mathf.PI));

            yield return null;
        }

        RectTransform.localPosition = targetCanvasPos;
        RectTransform.localScale = Vector3.one;
    }

}
