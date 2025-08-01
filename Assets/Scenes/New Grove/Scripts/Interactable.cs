using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Interaction : ScriptableObject
{
    public string interactionName;
    public bool unlocked;
    public int cost;

    public abstract void OnInteractionStart(Interactable interactable);
}

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string id;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2 spritePosition;
    [SerializeField] private Vector2 spriteSize;
    [SerializeField] private int level;
    [SerializeField] private int awakenCost = 5;
    [SerializeField] [TextArea] private string dialogue;

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
    [SerializeField] private FadeCanvasGroup touchIndicator;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference interactionView;

    public string Dialogue => dialogue;

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
