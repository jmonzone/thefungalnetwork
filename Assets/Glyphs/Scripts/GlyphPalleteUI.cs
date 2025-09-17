using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphPalleteUI : MonoBehaviour
{
    [SerializeField] private Transform glyphAnchor;
    [SerializeField] private GlyphDropZone glyphDropZone;
    [SerializeField] private GlyphButtonUI glyphPrefab;

    [SerializeField] private RectTransform glyphSlot1;
    [SerializeField] private RectTransform glyphSlot2;
    [SerializeField] private RectTransform glyphSlot3;

    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;
    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    [Header("Initial Glyphs")]
    [SerializeField] private DialogueGlyph glyph1;
    [SerializeField] private DialogueGlyph glyph2;
    [SerializeField] private DialogueGlyph glyph3;

    [Header("Glyph Images")]
    [SerializeField] private Sprite aceOfWands;
    [SerializeField] private Sprite aceOfCups;
    [SerializeField] private Sprite aceOfSpores;

    private DialogueGlyph targetGlyph;

    private List<GlyphButtonUI> glyphButtons = new List<GlyphButtonUI>();

    public event UnityAction OnGlyphReleased;

    private void Awake()
    {
        SpawnPalletteGlyph(glyph1, glyphSlot1.anchoredPosition);
        SpawnPalletteGlyph(glyph2, glyphSlot2.anchoredPosition);
        SpawnPalletteGlyph(glyph3, glyphSlot3.anchoredPosition);

        glyphDropZone.OnGlyphPlaced += HandleGlyphPlaced;
    }

    private void HandleGlyphPlaced(GlyphButtonUI placedGlyph, GlyphDropZone zone)
    {
        if (zone == glyphDropZone)
        {
            SpawnPalletteGlyph(placedGlyph.Glyph, placedGlyph.OriginalPosition);
        }
        else
        {
            Debug.Log("❌ Wrong glyph.");
            placedGlyph.ResetToOriginalParent();
        }
    }

    private void SpawnPalletteGlyph(DialogueGlyph glyph, Vector3 position)
    {
        var glyphObj = SpawnGlyph(glyph, position, glyphAnchor);
        glyphObj.OnGlyphDropped += OnGlyphSelected;

    }

    private GlyphButtonUI SpawnGlyph(DialogueGlyph glyph, Vector3 position, Transform parent)
    {
        var glyphObj = Instantiate(glyphPrefab, parent);
        glyphObj.GetComponent<RectTransform>().anchoredPosition = position;
        glyphObj.SetGlyph(glyph, GetGlyphSprite(glyph));

        glyphObj.OnGlyphFused += GlyphObj_OnGlyphFused;

        glyphButtons.Add(glyphObj);
        return glyphObj;
    }

    private void GlyphObj_OnGlyphFused(GlyphButtonUI dragged, GlyphButtonUI target)
    {
        // 1. Create a new fused glyph (this is your logic)
        DialogueGlyph fusedGlyph = DialogueGlyph.ACE_OF_SPORES;

        // 2. Destroy old glyphs
        Destroy(dragged.gameObject);
        Destroy(target.gameObject);

        SpawnGlyph(fusedGlyph, target.GetComponent<RectTransform>().anchoredPosition, glyphDropZone.transform);
    }


    private void OnGlyphSelected(GlyphButtonUI glyph)
    {
        if (targetGlyph == glyph.Glyph)
        {
            glyphImage.enabled = false;
            fungalText.colorGradient = normalColor;
            glyphAnchor.gameObject.SetActive(false);
            OnGlyphReleased?.Invoke();
        }
    }

    public void ShowGlyph(DialogueGlyph glyph)
    {
        targetGlyph = glyph;

        fungalText.colorGradient = blurColor;
        glyphAnchor.gameObject.SetActive(true);
        glyphImage.sprite = GetGlyphSprite(glyph);

        glyphImage.enabled = true;
    }

    private Sprite GetGlyphSprite(DialogueGlyph glyph) => glyph switch
    {
        DialogueGlyph.ACE_OF_WANDS => aceOfWands,
        DialogueGlyph.ACE_OF_CUPS => aceOfCups,
        DialogueGlyph.ACE_OF_SPORES => aceOfSpores,
        _ => aceOfWands,
    };
}
