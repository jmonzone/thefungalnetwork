using TMPro;
using UnityEngine;

public class GlyphPalleteSlotUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI countText;

    public RectTransform Rect => rectTransform;

    public void Initialize(int count)
    {
        countText.text = count.ToString();
    }
}
