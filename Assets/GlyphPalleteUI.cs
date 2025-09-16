using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphPalleteUI : MonoBehaviour
{
    [SerializeField] private Transform glyphAnchor;

    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;
    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    private DialogueGlyph targetGlyph;

    private List<GlyphButtonUI> glyphButtons = new List<GlyphButtonUI>();

    public event UnityAction OnGlyphReleased;

    private void Awake()
    {
        glyphAnchor.GetComponentsInChildren(includeInactive: true, glyphButtons);

        foreach (var button in glyphButtons)
        {
            button.OnGlyphClicked += () => OnGlyphSelected(button.Glyph);
        }
    }

    private void OnGlyphSelected(DialogueGlyph glyph)
    {
        if (targetGlyph == glyph)
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
        glyphImage.enabled = true;
        fungalText.colorGradient = blurColor;
        glyphAnchor.gameObject.SetActive(true);
    }
}
