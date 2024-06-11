using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject interactions;
    [SerializeField] private FeedPanel feedPanel;
    [SerializeField] private FungalInfoUI fungalInfoUI;

    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button escortButton;
    [SerializeField] private Button feedButton;
    [SerializeField] private TextMeshProUGUI escortButtonText;
    [SerializeField] private SlideAnimation slideAnimation;

    [SerializeField] private LookController player;

    private ProximityButtonManager proximityButtonManager;
    private FungalController fungal;

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
        closeButton.onClick.AddListener(() =>
        {
            StopFungalInteraction();
            SetState(UIState.JOYSTICK);
        });

        infoButton.onClick.AddListener(() => SetState(UIState.INFO));
        escortButton.onClick.AddListener(() =>
        {
            if (fungal.IsFollowing) fungal.Unescort();
            else fungal.Escort(player.transform);

            UpdateEscortButtonText();
        });

        proximityButtonManager = GetComponentInChildren<ProximityButtonManager>();
        proximityButtonManager.OnButtonClicked += entity =>
        {
            if (entity is FungalController fungal)
            {
                StartFungalInteraction(fungal);
            }
        };
        
        feedButton.onClick.AddListener(() => SetState(UIState.FEED));

        SetState(UIState.JOYSTICK);
    }

    private void StartFungalInteraction(FungalController fungal)
    {
        this.fungal = fungal;
        fungal.SetTarget(player.transform);

        var direction = fungal.transform.position - player.transform.position;
        direction.y = 0;
        player.Direction = direction;
        player.enabled = true;

        feedPanel.Fungal = fungal.Model;
        UpdateEscortButtonText();

        fungalInfoUI.SetFungal(fungal);
        SetState(UIState.INTERACTIONS);
    }

    private void StopFungalInteraction()
    {
        if (fungal)
        {
            player.enabled = false;
            if (!fungal.IsFollowing) fungal.SetTarget(null);
        }
    }

    private void UpdateEscortButtonText()
    {
        escortButtonText.text = fungal.IsFollowing ? "Unescort" : "Escort";
    }

    private void SetState(UIState state)
    {
        joystick.SetActive(state == UIState.JOYSTICK);
        inventory.SetActive(state == UIState.INVENTORY);
        interactions.SetActive(state == UIState.INTERACTIONS);
        fungalInfoUI.gameObject.SetActive(state == UIState.INFO);
        feedPanel.gameObject.SetActive(state == UIState.FEED);
        closeButton.gameObject.SetActive(state != UIState.JOYSTICK);
    }
}
