using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphPalleteUI : MonoBehaviour
{
    [SerializeField] private Transform glyphAnchor;
    [SerializeField] private GlyphDropZone glyphDropZone;
    [SerializeField] private GlyphController glyphPrefab;
    [SerializeField] private GlyphCollection glyphCollection;

    [SerializeField] private RectTransform glyphSlot1;
    [SerializeField] private RectTransform glyphSlot2;
    [SerializeField] private RectTransform glyphSlot3;

    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;
    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    [Header("Initial Glyphs")]
    [SerializeField] private GlyphData glyph1;
    [SerializeField] private GlyphData glyph2;
    [SerializeField] private GlyphData glyph3;

    private GlyphData targetGlyph;

    private List<GlyphController> glyphButtons = new List<GlyphController>();

    public event UnityAction OnGlyphReleased;

    private void Awake()
    {
        SpawnPalletteGlyph(glyph1, glyphSlot1.anchoredPosition);
        SpawnPalletteGlyph(glyph2, glyphSlot2.anchoredPosition);
        SpawnPalletteGlyph(glyph3, glyphSlot3.anchoredPosition);

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
        //glyphObj.OnGlyphDropped += OnGlyphSelected;

    }

    private GlyphController SpawnGlyph(GlyphData glyph, Vector3 position, bool isPaletteGlyph)
    {
        var glyphObj = Instantiate(glyphPrefab, isPaletteGlyph ? glyphAnchor : glyphDropZone.transform);
        glyphObj.GetComponent<RectTransform>().anchoredPosition = position;
        glyphObj.Initialize(glyph, isPaletteGlyph);

        glyphObj.OnGlyphFused += GlyphObj_OnGlyphFused;

        glyphButtons.Add(glyphObj);
        return glyphObj;
    }

    private void GlyphObj_OnGlyphFused(GlyphController dragged, GlyphController target)
    {
        if (glyphCollection.TryFuse(dragged.Glyph, target.Glyph, out GlyphData fusedGlyph))
        {
            Destroy(dragged.gameObject);
            Destroy(target.gameObject);

            SpawnGlyph(fusedGlyph, target.GetComponent<RectTransform>().anchoredPosition,false);
        }
    }

    private void OnGlyphSelected(GlyphController glyph)
    {
        if (targetGlyph == glyph.Glyph)
        {
            glyphImage.enabled = false;
            fungalText.colorGradient = normalColor;
            glyphAnchor.gameObject.SetActive(false);
            OnGlyphReleased?.Invoke();
        }
    }

    public void ShowGlyph(GlyphData glyph)
    {
        targetGlyph = glyph;

        fungalText.colorGradient = blurColor;
        glyphAnchor.gameObject.SetActive(true);
        glyphImage.sprite = glyph.Sprite;

        glyphImage.enabled = true;
    }
}
