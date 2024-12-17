using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuPartyPrepare : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Transform playerListAnchor;

    private List<TextMeshProUGUI> playerList = new List<TextMeshProUGUI>();

    private void Awake()
    {
        playerListAnchor.GetComponentsInChildren(true, playerList);
        multiplayerManager.OnLobbyPoll += MultiplayerManager_OnLobbyPoll;
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
