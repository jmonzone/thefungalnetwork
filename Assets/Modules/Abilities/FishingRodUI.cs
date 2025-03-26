using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image abilityBackground;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image fishIcon;
    [SerializeField] private TextMeshProUGUI abilityText;
    [SerializeField] private Color defaultBackgroundColor;

    private FungalThrow fungalTHrow;

    public void AssignFishingRod(FungalThrow fungalThrow)
    {
        // Ensure the previous listener is removed if the button was already assigned
        if (fungalTHrow != null)
        {
            fungalTHrow.OnFishChanged -= HandleFishChanged;
        }

        fungalTHrow = fungalThrow;

        // Subscribe to the events only if the button is not null
        if (fungalTHrow != null)
        {
            fungalTHrow.OnFishChanged += HandleFishChanged;
        }
        else
        {
            Debug.LogError("FishingRodButton is not assigned correctly.");
        }
    }

    private void OnDisable()
    {
        if (fungalTHrow != null)
        {
            fungalTHrow.OnFishChanged -= HandleFishChanged;
        }
    }

    private void HandleFishChanged(Fish fish)
    {
        if (fish != null)
        {
            abilityBackground.color = fish.BackgroundColor;
            abilityText.text = fish.AbilityName;
            fishIcon.sprite = fish.Icon;

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
