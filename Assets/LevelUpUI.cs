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

    public IEnumerator Show(UnitInstance instance, UnitSkill skill, UnityAction onComplete)
    {
        levelText.text = skill.Level.ToString();
        levelUpText.text = $"{instance.Data.Name} Leveled Up! How Sweet\nWhat's your dancing level now?";

        var milestones = skill.Milestones;
        if (milestones.Count > 0)
        {
            var firstMilestone = skill.Milestones[0];
            milestoneImage.sprite = firstMilestone.Sprite;
            milestoneName.text = firstMilestone.Label.ToString();
            milestoneDescription.text = firstMilestone.Description;
        }

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => onComplete?.Invoke());

        yield return fadeCanvasGroup.FadeIn();
    }

    public IEnumerator Hide()
    {
        yield return fadeCanvasGroup.FadeOut();
    }
}
