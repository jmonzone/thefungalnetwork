using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI pointsText;
    public Image outline;
    public Color clientColor;
    private Color defaultColor;

    private void Awake()
    {
        defaultColor = outline.color;
    }

    public void SetEntry(Sprite icon, string playerName, float points, bool isCLient)
    {
        if (iconImage != null) iconImage.sprite = icon;
        if (nameText != null) nameText.text = playerName;
        if (pointsText != null) pointsText.text = points.ToString();
        if (outline != null) outline.color = isCLient ? clientColor : defaultColor;
    }
}
