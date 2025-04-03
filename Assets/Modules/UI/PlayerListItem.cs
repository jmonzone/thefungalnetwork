using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private MultiplayerManager multiplayer;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerFungalImage;
    [SerializeField] private GameObject activeState;
    [SerializeField] private GameObject inactiveState;
    [SerializeField] private GameObject hostBadge;

    public void SetPlayer(Unity.Services.Lobbies.Models.Player player)
    {
        var fungalIndex = GetPlayerFungalIndex(player);
        var targetFungal = fungalCollection.Fungals[fungalIndex];

        playerNameText.text = player.Data.TryGetValue("PlayerName", out var playerNameData) ? playerNameData.Value : "Unknown Player";
        playerFungalImage.sprite = targetFungal.ActionImage;

        hostBadge.SetActive(multiplayer.JoinedLobby.HostId == player.Id);

        activeState.SetActive(true);
        inactiveState.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        activeState.SetActive(false);
        inactiveState.SetActive(true);
    }

    private int GetPlayerFungalIndex(Unity.Services.Lobbies.Models.Player player)
    {
        if (player.Data.TryGetValue("Fungal", out var fungalData) && int.TryParse(fungalData?.Value, out var index))
        {
            return Mathf.Clamp(index, 0, fungalCollection.Fungals.Count - 1);
        }
        return 0;
    }
}
