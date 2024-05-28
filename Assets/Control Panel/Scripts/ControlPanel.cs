using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject interactions;
    [SerializeField] private FeedPanel feedPanel;
    [SerializeField] private PetInfoManager petInfoManager;

    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button escortButton;
    [SerializeField] private Button feedButton;
    [SerializeField] private ActionButton actionButton;

    [SerializeField] private Transform player;

    private FungalController fungal;

    private enum State
    {
        JOYSTICK,
        INVENTORY,
        INTERACTIONS,
        INFO,
        FEED
    }

    private void Awake()
    {
        inventoryButton.onClick.AddListener(() => SetState(State.INVENTORY));
        closeButton.onClick.AddListener(() => SetState(State.JOYSTICK));
        infoButton.onClick.AddListener(() => SetState(State.INFO));
        escortButton.onClick.AddListener(() => fungal.SetTarget(player));
        actionButton.OnClicked += () => SetState(State.INTERACTIONS);
        feedButton.onClick.AddListener(() => SetState(State.FEED));

        SetState(State.JOYSTICK);
    }

    private void SetState(State state)
    {
        joystick.SetActive(state == State.JOYSTICK);
        inventory.SetActive(state == State.INVENTORY);
        interactions.SetActive(state == State.INTERACTIONS);
        petInfoManager.gameObject.SetActive(state == State.INFO);
        feedPanel.gameObject.SetActive(state == State.FEED);
        closeButton.gameObject.SetActive(state != State.JOYSTICK);
    }

    public void SetClosestFungalInteractions(FungalController fungal)
    {
        if (this.fungal == fungal) return;

        this.fungal = fungal;

        if (fungal)
        {
            feedPanel.Fungal = fungal.FungalInstance;
            actionButton.SetInteraction(fungal.FungalInstance.Data.ActionImage, fungal.FungalInstance.Data.Color);
            petInfoManager.SetFungal(fungal.FungalInstance);
        }

        actionButton.SetVisible(fungal);
    }
}
