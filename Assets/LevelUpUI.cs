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

    [SerializeField] private Image milestoneImage;
    [SerializeField] private TextMeshProUGUI milestoneName;
    [SerializeField] private TextMeshProUGUI milestoneDescription;

    [SerializeField] private Button backButton;

    public event UnityAction OnExit;

    private void Awake()
    {
        backButton.onClick.AddListener(() => OnExit?.Invoke());
    }

    public IEnumerator Show(UnitInstance instance, UnitSkill skill, DanceMoveInstance unlock)
    {
        levelText.text = skill.Level.ToString();
        levelUpText.text = $"{instance.Data.Name} Leveled Up! How Sweet\nWhat's your dancing level now?";

        milestoneImage.sprite = unlock.Data.Sprite;
        milestoneName.text = unlock.Label.ToString();
        milestoneDescription.text = unlock.Description;

        yield return fadeCanvasGroup.FadeIn();
    }

    public IEnumerator Hide()
    {
        yield return fadeCanvasGroup.FadeOut();
    }
}
