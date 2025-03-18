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
    public TextMeshProUGUI badgeText;
    public Color tiedColor;
    public Color mvpColor;

    private void Awake()
    {
        defaultColor = outline.color;
    }

    public void SetEntry(Sprite icon, string playerName, float points, bool isCLient, bool isTopPlayer, bool isTiedAtTop)
    {
        if (iconImage != null) iconImage.sprite = icon;
        if (nameText != null) nameText.text = playerName;
        if (pointsText != null) pointsText.text = points.ToString();
        if (outline != null) outline.color = isCLient ? clientColor : defaultColor;

        /// MVP / TIE badge logic
        if (isTiedAtTop)
        {
            badgeText.gameObject.SetActive(true);
            badgeText.text = "TIE";
            badgeText.color = tiedColor;

        }
        else if (isTopPlayer)
        {
            badgeText.gameObject.SetActive(true);

            badgeText.text = "MVP";
            badgeText.color = mvpColor;
        }
        else
        {
            badgeText.gameObject.SetActive(false);
        }
    }
}
