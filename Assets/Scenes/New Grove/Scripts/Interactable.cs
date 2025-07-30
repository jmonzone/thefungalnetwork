using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Interaction
{
    public string interactionName;
}

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string id;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2 spritePosition;
    [SerializeField] private Vector2 spriteSize;
    [SerializeField] private int level;
    [SerializeField] private int awakenCost = 5;

    [SerializeField] private List<Interaction> interactions;

    public string Id => id;
    public Sprite Sprite => sprite;
    public Vector2 SpritePosition => spritePosition;
    public Vector2 SpriteSize => spriteSize;
    public int Level => level;
    public int AwakenCost => awakenCost;

    public List<Interaction> Interactions => interactions;

    public Transform Transform => transform;

    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private TextMeshProUGUI dialogueHeaderText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private FadeCanvasGroup touchIndicator;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference interactionView;

    [SerializeField] [TextArea] private string level1Dialogue;
    [SerializeField] [TextArea] private string level2Dialogue;

    private void Awake()
    {
        inventoryController.OnMushroomCountChanged += InventoryController_OnMushroomCountChanged;
    }

    private void InventoryController_OnMushroomCountChanged(int count)
    {
        if (level == 0 && count >= awakenCost && !touchIndicator.IsVisible)
        {
            StartCoroutine(touchIndicator.FadeIn());
            inventoryController.OnMushroomCountChanged -= InventoryController_OnMushroomCountChanged;
        }
    }

    public void OnBaseInteraction()
    {
        StopAllCoroutines();

        if (level == 0)
        {
            dialogueHeaderText.text = "???";
            dialogueText.text = level1Dialogue;
        }
        else
        {
            dialogueHeaderText.text = id;
            dialogueText.text = level2Dialogue;
        }

        cameraPanController.CenterTargetInView(transform);

        if (touchIndicator.gameObject.activeSelf) StartCoroutine(touchIndicator.FadeOut());
        if (navigation.CurrentView != interactionView)
        {
            navigation.Navigate(interactionView);
        }
    }

    public void Awaken()
    {
        IncreaseLevel();
    }

    public void IncreaseLevel()
    {
        level++;
    }
}
