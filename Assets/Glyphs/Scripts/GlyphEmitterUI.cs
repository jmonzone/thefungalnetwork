using System.Collections.Generic;
using UnityEngine;

public class GlyphEmitterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas uiCanvas; // Your main UI canvas
    [SerializeField] private RectTransform targetContainer;
    [SerializeField] private BlockingGlyphController blockingGlyphPrefab;

    [Header("Settings")]
    [SerializeField] private float duration = 1f;       // travel time
    [SerializeField] private float scalePulse = 0.2f;   // bounce factor while animating

    [SerializeField] private List<BlockingGlyphController> glyphControllers = new List<BlockingGlyphController>();
    public List<BlockingGlyphController> GlyphControllers => glyphControllers;

    public void EmitGlyphs(List<GlyphData> glyphs, List<string> words, Vector3 mouthWorldPosition)
    {
        glyphControllers = new List<BlockingGlyphController>();

        int count = glyphs.Count;
        float containerWidth = targetContainer.rect.width;

        // Convert mouth world position → canvas local
        Vector3 startCanvasPos = WorldToCanvasLocal(uiCanvas, mouthWorldPosition);

        Vector3 containerCenterCanvas = targetContainer.position;
        Vector2 containerCenterCanvasLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, containerCenterCanvas),
            null, // Overlay canvas ignores camera
            out containerCenterCanvasLocal
        );

        // Even spacing with two imaginary glyphs at the edges
        float spacing = containerWidth / (count + 1);

        for (int i = 0; i < count; i++)
        {
            GlyphData glyph = glyphs[i];

            // Position between the "imaginary" edges
            float offsetX = spacing * (i + 1) - containerWidth / 2f;

            Vector3 targetCanvasPos = (Vector3)containerCenterCanvasLocal + new Vector3(offsetX, 0, 0);

            var blockingGlyph = Instantiate(blockingGlyphPrefab, uiCanvas.transform);
            blockingGlyph.Initialize(glyph, words, targetContainer);
            glyphControllers.Add(blockingGlyph);

            StartCoroutine(blockingGlyph.Animate(startCanvasPos, targetCanvasPos, duration, scalePulse));
        }
    }

    private Vector3 WorldToCanvasLocal(Canvas canvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null, // Overlay canvas ignores camera
            out localPoint
        );

        return new Vector3(localPoint.x, localPoint.y, 0f);
    }
}
