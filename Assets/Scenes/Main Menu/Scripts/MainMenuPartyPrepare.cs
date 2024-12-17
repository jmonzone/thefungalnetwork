using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPartyPrepare : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Transform playerListAnchor;
    [SerializeField] private Button exitButton;
    [SerializeField] private Navigation navigation;

    private List<TextMeshProUGUI> playerList = new List<TextMeshProUGUI>();

    private void Awake()
    {
        playerListAnchor.GetComponentsInChildren(true, playerList);
        multiplayerManager.OnLobbyPoll += MultiplayerManager_OnLobbyPoll;

        exitButton.onClick.AddListener(async () =>
        {
            await multiplayerManager.LeaveLobby();
            navigation.GoBack();
        });
    }

    private void MultiplayerManager_OnLobbyPoll()
    {
        var players = multiplayerManager.JoinedLobby.Players;
        for(var i = 0; i < players.Count; i++)
        {
            playerList[i].text = players[i].Data["PlayerName"].Value;
            playerList[i].gameObject.SetActive(true);
        }

        for (var i = players.Count; i < playerList.Count; i++)
        {
            playerList[i].gameObject.SetActive(false);
        }
    }
}
