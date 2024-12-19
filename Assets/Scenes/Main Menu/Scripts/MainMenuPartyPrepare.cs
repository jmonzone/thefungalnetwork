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

        startButton.onClick.AddListener(() =>
        {
            sceneNavigation.NavigateToScene(4);
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
        multiplayerManager.OnLobbyJoined += MultiplayerManager_OnLobbyJoined;
    }

    private void OnDisable()
    {
        multiplayerManager.OnLobbyPoll -= MultiplayerManager_OnLobbyPoll;
        multiplayerManager.OnLobbyJoined -= MultiplayerManager_OnLobbyJoined;
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

        if (!joining && MultiplayerManager.Instance.JoinedLobby.Data.ContainsKey("JoinCode"))
        {
            Debug.Log("joining");
            sceneNavigation.NavigateToScene(4);
            joining = true;
        }
    }

    private bool joining = false;

    private void MultiplayerManager_OnLobbyJoined()
    {
        startButton.gameObject.SetActive(MultiplayerManager.Instance.IsHost);
    }
}
