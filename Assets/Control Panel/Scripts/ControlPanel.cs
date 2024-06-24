using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    public event UnityAction OnEscortButtonClicked;
    public event UnityAction OnFungalInteractionEnd;

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
            OnFungalInteractionEnd?.Invoke();
            SetState(UIState.JOYSTICK);
        });

        infoButton.onClick.AddListener(() => SetState(UIState.INFO));
        escortButton.onClick.AddListener(() =>
        {
            OnEscortButtonClicked?.Invoke();
            UpdateEscortButtonText();
        });
        
        feedButton.onClick.AddListener(() => SetState(UIState.FEED));

        fungalInfoUI.OnClose += () => SetState(UIState.INTERACTIONS);

        SetState(UIState.JOYSTICK);
    }

    public void SetFungal(FungalController fungal)
    {
        this.fungal = fungal;
        fungalInfoUI.SetFungal(fungal);
        feedPanel.Fungal = fungal.Model;
        UpdateEscortButtonText();
        SetState(UIState.INTERACTIONS);
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

    public void SetVisible(bool visible)
    {
        slideAnimation.IsVisible = visible;
    }
}
