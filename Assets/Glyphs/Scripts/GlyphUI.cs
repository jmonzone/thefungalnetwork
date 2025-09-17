using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;
    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    [Header("Initial Glyphs")]
    [SerializeField] private GlyphData glyph1;
    [SerializeField] private GlyphData glyph2;
    [SerializeField] private GlyphData glyph3;
    [SerializeField] private GlyphData glyph4;

    private GlyphData targetGlyph;

    private List<GlyphController> glyphControllers = new List<GlyphController>();

    public event UnityAction OnGlyphMatched;

    private void Awake()
    {
        SpawnPalletteGlyph(glyph1, glyphSlot1.anchoredPosition);
        SpawnPalletteGlyph(glyph2, glyphSlot2.anchoredPosition);
        SpawnPalletteGlyph(glyph3, glyphSlot3.anchoredPosition);
        SpawnPalletteGlyph(glyph4, glyphSlot4.anchoredPosition);

        glyphDropZone.OnGlyphPlaced += HandleGlyphPlaced;
    }

    private void HandleGlyphPlaced(GlyphController placedGlyph, GlyphDropZone zone)
    {
        if (zone == glyphDropZone)
        {
            if (placedGlyph.IsPalleteGlyph)
            {
                SpawnPalletteGlyph(placedGlyph.Glyph, placedGlyph.OriginalPosition);
            }
        }
        else
        {
            placedGlyph.ReturnToOriginalParent();
        }
    }

    private void SpawnPalletteGlyph(GlyphData glyph, Vector3 position)
    {
        var glyphObj = SpawnGlyph(glyph, position, true);
        glyphObj.OnGlyphDropped += OnGlyphFused;

    }

    private GlyphController SpawnGlyph(GlyphData glyph, Vector3 position, bool isPaletteGlyph)
    {
        var glyphObj = Instantiate(glyphPrefab, isPaletteGlyph ? glyphPalette : glyphDropZone.transform);
        glyphObj.GetComponent<RectTransform>().anchoredPosition = position;
        glyphObj.Initialize(glyph, isPaletteGlyph);

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

            var fusedGlyphController = SpawnGlyph(fusedGlyph, target.GetComponent<RectTransform>().anchoredPosition,false);
            OnGlyphFused(fusedGlyphController);
        }
    }

    private void OnGlyphFused(GlyphController glyph)
    {
        if (targetGlyph == glyph.Glyph)
        {
            glyphImage.enabled = false;
            fungalText.colorGradient = normalColor;
            glyphPalette.gameObject.SetActive(false);
            OnGlyphMatched?.Invoke();
        }
    }

    public void ShowGlyph(GlyphData glyph)
    {
        targetGlyph = glyph;

        fungalText.colorGradient = blurColor;

        glyphImage.sprite = glyph.Sprite;
        glyphImage.enabled = true;

        glyphPalette.gameObject.SetActive(true);
        glyphDropZone.gameObject.SetActive(true);
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

        glyphImage.enabled = false;
        glyphPalette.gameObject.SetActive(false);
        glyphDropZone.gameObject.SetActive(false);
    }
}
