using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private List<InteractionButton> interactionButtons;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference interactionView;
    [SerializeField] private ViewReference dialogueView;
    [SerializeField] private TextMeshProUGUI levelText;

    private int level = 0;

    public event UnityAction OnButtonUnlockable;

    private void Awake()
    {
        interactionController.OnInteractionStart += _ =>
        {
            if (navigation.CurrentView == interactionView) navigation.GoBack();
        };

        inventoryController.OnMushroomCountChanged += InventoryController_OnMushroomCountChanged;

        levelText.text = "Bogged Down";

        foreach (var button in interactionButtons)
        {
            button.OnUnlocked += () =>
            {
                inventoryController.SetMushroomCount(inventoryController.MushroomCount - button.Cost);
                level++;
                levelText.text = $"Level {level}";
            };
        }
    }

    private void InventoryController_OnMushroomCountChanged(int count)
    {
        foreach(var button in interactionButtons)
        {
            if (!button.Unlocked && count >= button.Cost)
            {
                Debug.Log("OnButtonUnlockable");
                OnButtonUnlockable?.Invoke();
            }
            button.SetInteractable(button.Unlocked || count >= button.Cost);
        }
    }

}
