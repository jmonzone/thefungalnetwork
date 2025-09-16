using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphButtonUI : MonoBehaviour
{
    [SerializeField] private Image glyphImage;
    [SerializeField] private Button button;
    [SerializeField] private DialogueGlyph glyph;

    public DialogueGlyph Glyph => glyph;

    public event UnityAction OnGlyphClicked;

    private void Awake()
    {
        button.onClick.AddListener(() => OnGlyphClicked?.Invoke());
    }

    public void SetGlyph(DialogueGlyph glyph, Sprite sprite)
    {
        this.glyph = glyph;
        glyphImage.sprite = sprite;
    }
}
