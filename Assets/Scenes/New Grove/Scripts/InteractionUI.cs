using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference interactionView;
    [SerializeField] private ViewReference dialogueView;

    [SerializeField] private Image interactableImage;
    [SerializeField] private TextMeshProUGUI interactableNameText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private Button awakenButton;
    [SerializeField] private TextMeshProUGUI awakenCostText;

    [SerializeField] private GameObject interactionButtonsContainer;

    private List<InteractionButton> interactionButtons = new List<InteractionButton>();

    private Interactable interactable;

    public event UnityAction OnButtonUnlockable;

    private void Awake()
    {
        interactionButtonsContainer.GetComponentsInChildren(true, interactionButtons);

        interactionController.OnInteractableSelected += InteractionController_OnInteractableSelected;
        interactionController.OnGroundSelected += _ =>
        {
            if (navigation.CurrentView == interactionView) navigation.GoBack();
        };

        inventoryController.OnMushroomCountChanged += InventoryController_OnMushroomCountChanged;

        foreach (var button in interactionButtons)
        {
            button.OnInteractionClicked += () => button.Interaction.OnInteractionStart(interactable);

            button.OnUnlocked += () =>
            {
                inventoryController.SetMushroomCount(inventoryController.MushroomCount - button.Interaction.cost);
                interactable.IncreaseLevel();
                levelText.text = $"Level {interactable.Level}";
            };
        }

        awakenButton.onClick.AddListener(() =>
        {
            if (interactable)
            {
                inventoryController.SetMushroomCount(inventoryController.MushroomCount - interactable.AwakenCost);
                interactable.Awaken();
                UpdateView();
            }
        });
    }

    private void InteractionController_OnInteractableSelected(Interactable interactable)
    {
        this.interactable = interactable;

        UpdateView();
    }

    private void UpdateView()
    {
        if (!interactable) return;

        interactableNameText.text = interactable.Id;
        interactableImage.sprite = interactable.Sprite;
        interactableImage.rectTransform.anchoredPosition = interactable.SpritePosition;
        interactableImage.rectTransform.sizeDelta = interactable.SpriteSize;

        if (interactable.Level == 0)
        {
            levelText.text = $"Asleep";
            awakenButton.interactable = inventoryController.MushroomCount >= interactable.AwakenCost;
            awakenCostText.text = interactable.AwakenCost.ToString();

            awakenButton.gameObject.SetActive(true);
            interactionButtonsContainer.SetActive(false);
        }
        else
        {
            levelText.text = $"Level {interactable.Level}";
            awakenButton.gameObject.SetActive(false);
            interactionButtonsContainer.SetActive(true);
        }

        for (var i = 0; i < interactionButtons.Count; i++)
        {
            if (interactable.Interactions.Count > i)
            {
                var interaction = interactable.Interactions[i];
                interactionButtons[i].SetInteraction(interaction);
                interactionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                interactionButtons[i].gameObject.SetActive(false);
            }
        }
    }


    private void InventoryController_OnMushroomCountChanged(int count)
    {
        if (interactable)
        {
            awakenButton.interactable = inventoryController.MushroomCount >= interactable.AwakenCost;
        }
    }
}
