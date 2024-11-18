using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image image;

    public void Show(int level, Upgrade upgrade)
    {
        levelText.text = $"You've just advanced a Fishing level!\nYou have now reached level {level}.";
        bodyText.text = upgrade.Description;
        image.sprite = upgrade.Sprite;
        Show();
    }
}
