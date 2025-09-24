using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlyphCountUI : MonoBehaviour
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;

    public void SetGlyphCount(GlyphData glyph)
    {
        if (inventory.Glyphs.ContainsKey(glyph))
        {
            text.text = inventory.Glyphs[glyph].ToString();
        }
        else
        {
            text.text = "0";
        }

        image.sprite = glyph.Sprite;
    }
}
