using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FungalThrowUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image abilityOutline;
    [SerializeField] private Image abilityBackground;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image fishIcon;
    [SerializeField] private TextMeshProUGUI abilityText;
    [SerializeField] private Color defaultBackgroundColor;

    public  void UpdateView(FishController fish)
    {
        Debug.Log($"HandleFishChanged {fish?.name ?? null}");
        if (fish != null)
        {
            abilityBackground.color = fish.Ability.BackgroundColor;
            abilityText.text = fish.Ability.Id;
            fishIcon.sprite = fish.Ability.Image;

            abilityIcon.gameObject.SetActive(false);
            fishIcon.gameObject.SetActive(true);
        }
        else
        {
            abilityBackground.color = defaultBackgroundColor;
            abilityIcon.gameObject.SetActive(true);
            fishIcon.gameObject.SetActive(false);
            abilityText.text = "No Fish Available";
        }
    }
}
