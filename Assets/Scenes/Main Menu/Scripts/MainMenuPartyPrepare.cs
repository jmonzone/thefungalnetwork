using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPartyPrepare : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Transform playerListAnchor;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private Navigation navigation;

    private List<TextMeshProUGUI> playerList = new List<TextMeshProUGUI>();

    private void Awake()
    {
        playerListAnchor.GetComponentsInChildren(true, playerList);

        startButton.onClick.AddListener(async () =>
        {
            await multiplayerManager.CreateRelay();
            sceneNavigation.NavigateToScene(5);

        });

        exitButton.onClick.AddListener(async () =>
        {
            await multiplayerManager.LeaveLobby();
            navigation.GoBack();
        });
    }

    private void OnEnable()
    {
        multiplayerManager.OnLobbyPoll += MultiplayerManager_OnLobbyPoll;
    }

    private void OnDisable()
    {
        multiplayerManager.OnLobbyPoll -= MultiplayerManager_OnLobbyPoll;
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
