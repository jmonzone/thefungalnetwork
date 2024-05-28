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
    private UIState currentState;

    private enum UIState
    {
        JOYSTICK,
        INVENTORY,
        INTERACTIONS,
        INFO,
        FEED
    }

    private void Awake()
    {
        inventoryButton.onClick.AddListener(() => SetState(UIState.INVENTORY));
        closeButton.onClick.AddListener(() => SetState(UIState.JOYSTICK));
        infoButton.onClick.AddListener(() => SetState(UIState.INFO));
        escortButton.onClick.AddListener(() => fungal.SetTarget(player));
        actionButton.OnClicked += () => SetState(UIState.INTERACTIONS);
        feedButton.onClick.AddListener(() => SetState(UIState.FEED));

        SetState(UIState.JOYSTICK);
    }

    private void SetState(UIState state)
    {
        currentState = state;

        joystick.SetActive(state == UIState.JOYSTICK);
        inventory.SetActive(state == UIState.INVENTORY);
        interactions.SetActive(state == UIState.INTERACTIONS);
        petInfoManager.gameObject.SetActive(state == UIState.INFO);
        feedPanel.gameObject.SetActive(state == UIState.FEED);
        closeButton.gameObject.SetActive(state != UIState.JOYSTICK);
    }

    public void SetClosestFungalInteractions(FungalController fungal)
    {
        if (this.fungal == fungal || currentState == UIState.INFO) return;

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
