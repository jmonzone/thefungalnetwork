using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private MultiplayerReference multiplayer;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerFungalImage;
    [SerializeField] private GameObject activeState;
    [SerializeField] private GameObject inactiveState;
    [SerializeField] private GameObject hostBadge;

    [SerializeField] private Image removeButtonImage; 
    [SerializeField] private Button removeButton;

    private LobbyPlayer player;

    private void Awake()
    {
        removeButton.onClick.AddListener(async () =>
        {
            //Debug.Log("removeButton click");
            removeButton.interactable = false;
            await multiplayer.RemoveAIPlayer(player);
            removeButton.interactable = true;
        });
    }

    public void SetPlayer(LobbyPlayer player)
    {
        this.player = player;

        playerNameText.text = player.name.ToString();

        var targetFungal = fungalCollection.Fungals[player.fungal];
        playerFungalImage.sprite = targetFungal.ActionImage;

        hostBadge.SetActive(player.isHost);

        activeState.SetActive(true);
        inactiveState.SetActive(false);
        gameObject.SetActive(true);

        removeButton.enabled = player.isAI;
        removeButtonImage.enabled = player.isAI;
    }

    public void Reset()
    {
        activeState.SetActive(false);
        inactiveState.SetActive(true);
    }
}
