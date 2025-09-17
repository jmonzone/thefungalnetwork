using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GlyphDropZone : MonoBehaviour, IDropHandler
{
    public event UnityAction<GlyphButtonUI, GlyphDropZone> OnGlyphPlaced;

    public void OnDrop(PointerEventData eventData)
    {
        var glyphUI = eventData.pointerDrag.GetComponent<GlyphButtonUI>();
        if (glyphUI != null)
        {
            // Snap into place
            glyphUI.transform.SetParent(transform);
            // Notify listeners that a glyph was placed
            OnGlyphPlaced?.Invoke(glyphUI, this);
        }
    }
}
