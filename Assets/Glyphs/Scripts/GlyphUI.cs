using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlyphUI : MonoBehaviour
{
    [SerializeField] private Transform glyphPalette;
    [SerializeField] private GlyphDropZone glyphDropZone;
    [SerializeField] private GlyphController glyphPrefab;
    [SerializeField] private GlyphCollection glyphCollection;

    [SerializeField] private RectTransform glyphSlot1;
    [SerializeField] private RectTransform glyphSlot2;
    [SerializeField] private RectTransform glyphSlot3;
    [SerializeField] private RectTransform glyphSlot4;

    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;

    [Header("Initial Glyphs")]
    [SerializeField] private GlyphData glyph1;
    [SerializeField] private GlyphData glyph2;
    [SerializeField] private GlyphData glyph3;
    [SerializeField] private GlyphData glyph4;

    private List<GlyphController> glyphControllers = new List<GlyphController>();

    public event UnityAction<GlyphController> OnGlyphFused;

    private void Awake()
    {
        SpawnGlyph(glyph1, glyphSlot1.anchoredPosition, isPaletteGlyph: true, new List<string>());
        SpawnGlyph(glyph2, glyphSlot2.anchoredPosition, isPaletteGlyph: true, new List<string>());
        SpawnGlyph(glyph3, glyphSlot3.anchoredPosition, isPaletteGlyph: true, new List<string>());
        SpawnGlyph(glyph4, glyphSlot4.anchoredPosition, isPaletteGlyph: true, new List<string>());

        glyphDropZone.OnGlyphPlaced += HandleGlyphPlaced;
    }

    private void HandleGlyphPlaced(GlyphController placedGlyph, GlyphDropZone zone)
    {
        if (zone == glyphDropZone)
        {
            if (placedGlyph.IsPalleteGlyph)
            {
                SpawnGlyph(placedGlyph.Glyph, placedGlyph.OriginalPosition, true, new List<string>());
            }
        }
        else
        {
            placedGlyph.ReturnToOriginalParent();
        }
    }

    private GlyphController SpawnGlyph(GlyphData glyph, Vector3 position, bool isPaletteGlyph, List<string> words)
    {
        var glyphObj = Instantiate(glyphPrefab, isPaletteGlyph ? glyphPalette : glyphDropZone.transform);
        glyphObj.GetComponent<RectTransform>().anchoredPosition = position;
        glyphObj.Initialize(glyph, isPaletteGlyph, words );

        glyphObj.OnGlyphFused += GlyphObj_OnGlyphFused;

        glyphControllers.Add(glyphObj);
        return glyphObj;
    }

    private void GlyphObj_OnGlyphFused(GlyphController dragged, GlyphController target)
    {
        if (glyphCollection.TryFuse(dragged.Glyph, target.Glyph, out GlyphData fusedGlyph))
        {
            glyphControllers.Remove(dragged);
            Destroy(dragged.gameObject);

            glyphControllers.Remove(target);
            Destroy(target.gameObject);

            var targetRect = target.GetComponent<RectTransform>();
            var fusedGlyphController = SpawnGlyph(fusedGlyph, targetRect.anchoredPosition, false, new List<string>());

            OnGlyphFused?.Invoke(fusedGlyphController);
        }
        else
        {
            Debug.Log("no fusion");
        }
    }

    public void HideGlyphUI()
    {
        foreach(var glyph in glyphControllers)
        {
            if (!glyph.IsPalleteGlyph)
            {
                Destroy(glyph.gameObject);
            }
        }

        glyphControllers = new List<GlyphController>();
    }
}
