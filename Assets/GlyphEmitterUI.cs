using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GlyphEmitterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas uiCanvas; // Your main UI canvas
    [SerializeField] private RectTransform targetContainer;
    [SerializeField] private GlyphController glyphController;

    [Header("Settings")]
    [SerializeField] private float duration = 1f;       // travel time
    [SerializeField] private float scalePulse = 0.2f;   // bounce factor while animating

    public void EmitGlyphs(List<GlyphData> glyphs, Vector3 mouthWorldPosition)
    {
        int count = glyphs.Count;
        float containerWidth = targetContainer.rect.width;

        // Convert mouth world position → canvas local
        Vector3 startCanvasPos = WorldToCanvasLocal(uiCanvas, mouthWorldPosition);
        Debug.Log($"{startCanvasPos} {mouthWorldPosition}");

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

            StartCoroutine(AnimateGlyphCanvas(glyph, startCanvasPos, targetCanvasPos));
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



    private IEnumerator AnimateGlyphCanvas(GlyphData glyph, Vector3 startCanvasPos, Vector3 targetCanvasPos)
    {
        var glyphGO = Instantiate(glyphController, uiCanvas.transform);
        glyphGO.RectTransform.localPosition = startCanvasPos;
        glyphGO.Initialize(glyph, isPalleteGlyph: false);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            glyphGO.RectTransform.localPosition = Vector3.Lerp(
                startCanvasPos,
                targetCanvasPos,
                Mathf.SmoothStep(0, 1, normalized)
            );

            glyphGO.RectTransform.localScale = Vector3.one * (1f + scalePulse * Mathf.Sin(normalized * Mathf.PI));

            yield return null;
        }

        glyphGO.RectTransform.localPosition = targetCanvasPos;
        glyphGO.RectTransform.localScale = Vector3.one;
    }


}
