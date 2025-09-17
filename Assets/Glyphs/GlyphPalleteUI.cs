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


    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;
    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    [Header("Glyph Images")]
    [SerializeField] private Sprite aceOfWands;
    [SerializeField] private Sprite aceOfCups;
    [SerializeField] private Sprite aceOfSpores;

    private DialogueGlyph targetGlyph;

    private List<GlyphButtonUI> glyphButtons = new List<GlyphButtonUI>();

    public event UnityAction OnGlyphReleased;

    private void Awake()
    {
        glyphAnchor.GetComponentsInChildren(includeInactive: true, glyphButtons);

        foreach (var button in glyphButtons)
        {
            button.OnGlyphDropped += OnGlyphSelected;
        }

        glyphDropZone.OnGlyphPlaced += HandleGlyphPlaced;
    }

    private void HandleGlyphPlaced(GlyphButtonUI placedGlyph, GlyphDropZone zone)
    {
        if (zone == glyphDropZone)
        {
            Debug.Log("✅ Correct glyph placed!");
            var replacementGlyph = Instantiate(glyphPrefab, glyphAnchor);
            replacementGlyph.GetComponent<RectTransform>().anchoredPosition = placedGlyph.OriginalPosition;
            replacementGlyph.SetGlyph(placedGlyph.Glyph, GetGlyphSprite(placedGlyph.Glyph));
        }
        else
        {
            Debug.Log("❌ Wrong glyph.");
            placedGlyph.ResetToOriginalParent();
        }
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
