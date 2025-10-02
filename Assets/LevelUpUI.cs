using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private Image unlockImage;
    [SerializeField] private TextMeshProUGUI unlockNameText;
    [SerializeField] private TextMeshProUGUI unlockDescriptionText;
    [SerializeField] private Button backButton;

    public event UnityAction OnExit;

    private void Awake()
    {
        backButton.onClick.AddListener(() => OnExit?.Invoke());
    }

    public IEnumerator Show(UnitInstance instance, UnitSkill skill, DanceMove unlock)
    {
        levelText.text = skill.Level.ToString();
        levelUpText.text = $"{instance.Data.Name} Leveled Up! How Sweet\nWhat's your dancing level now?";

        unlockImage.sprite = unlock.Sprite;
        unlockNameText.text = unlock.Label.ToString();

        yield return fadeCanvasGroup.FadeIn();
    }

    public IEnumerator Hide()
    {
        yield return fadeCanvasGroup.FadeOut();
    }
}
