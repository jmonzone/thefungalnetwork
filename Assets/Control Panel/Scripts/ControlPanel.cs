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

    [SerializeField] private PlayerController player;

    public FungalController EscortedFungal { get; private set; }
    public PlayerController Player => player;

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
            player.EndTalk();
            SetState(UIState.JOYSTICK);
        });

        infoButton.onClick.AddListener(() => SetState(UIState.INFO));
        escortButton.onClick.AddListener(() =>
        {
            if (EscortedFungal)
            {
                UnescortFungal();
            }
            else
            {
                EscortedFungal = player.TalkingFungal;
                EscortedFungal.Escort(player.transform);
            }

            UpdateEscortButtonText();
        });
        
        feedButton.onClick.AddListener(() => SetState(UIState.FEED));

        SetState(UIState.JOYSTICK);
    }

    public void UnescortFungal()
    {
        if (EscortedFungal)
        {
            EscortedFungal.Unescort();
            EscortedFungal = null;
        }
    }

    public void StartFungalInteraction(FungalController fungal)
    {
        feedPanel.Fungal = fungal.Model;
        UpdateEscortButtonText();

        fungalInfoUI.SetFungal(fungal);
        SetState(UIState.INTERACTIONS);
    }

    private void UpdateEscortButtonText()
    {
        escortButtonText.text = player.TalkingFungal.IsFollowing ? "Unescort" : "Escort";
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

    public void SetVisible(bool visible)
    {
        slideAnimation.IsVisible = visible;
    }
}
