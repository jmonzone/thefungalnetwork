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

    [SerializeField] private GameObject milestoneContainer;
    [SerializeField] private Image milestoneImage;
    [SerializeField] private TextMeshProUGUI milestoneName;
    [SerializeField] private TextMeshProUGUI milestoneDescription;

    [SerializeField] private Button backButton;

    public IEnumerator Show(ActivityUnit unit, UnityAction onComplete)
    {
        levelText.text = unit.Skill.Level.ToString();
        levelUpText.text = $"{unit.Name} Leveled Up! How Sweet\nWhat's your {unit.Skill.Label} level now?";

        var milestones = unit.Skill.Milestones;
        milestoneContainer.SetActive(milestones.Count > 0);
        if (milestones.Count > 0)
        {
            var firstMilestone = unit.Skill.Milestones[0];
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
