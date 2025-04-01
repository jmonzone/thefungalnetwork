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
    [SerializeField] private Color winnerColor;

    private void Awake()
    {
        defaultColor = outline.color;
    }

    public void SetEliminationEntry(Sprite icon, string playerName, string kills, bool showBadge)
    {
        if (iconImage != null) iconImage.sprite = icon;
        if (nameText != null) nameText.text = playerName;
        if (pointsText != null) pointsText.text = kills;
        if (badgeText != null)
        {
            badgeText.gameObject.SetActive(showBadge);
            badgeText.text = "WINNER";
            badgeText.color = winnerColor;
        }
    }

    public void SetPartyEntry(Sprite icon, string playerName, string score, bool isCLient, bool isTopPlayer, bool isTiedAtTop)
    {
        if (iconImage != null) iconImage.sprite = icon;
        if (nameText != null) nameText.text = playerName;
        if (pointsText != null) pointsText.text = score;
        //if (outline != null) outline.color = isCLient ? clientColor : defaultColor;

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
