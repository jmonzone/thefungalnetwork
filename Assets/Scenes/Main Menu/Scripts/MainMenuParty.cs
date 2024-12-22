using System.Linq;
using GURU;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuParty : MonoBehaviour
{
    [SerializeField] private ListUI playerList;
    [SerializeField] private ListUI fungalList;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private Navigation navigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private MultiplayerManager multiplayer;

    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {
            sceneNavigation.NavigateToScene(4);
        });

        exitButton.onClick.AddListener(async () =>
        {
            await multiplayer.LeaveLobby();
            navigation.GoBack();
        });
    }

    private void OnEnable()
    {
        MultiplayerManager_OnLobbyJoined();
        multiplayer.OnLobbyPoll += MultiplayerManager_OnLobbyPoll;
        multiplayer.OnLobbyJoined += MultiplayerManager_OnLobbyJoined;
    }

    private void OnDisable()
    {
        multiplayer.OnLobbyPoll -= MultiplayerManager_OnLobbyPoll;
        multiplayer.OnLobbyJoined -= MultiplayerManager_OnLobbyJoined;
    }

    private void MultiplayerManager_OnLobbyPoll()
    {
        var players = multiplayer.JoinedLobby.Players;

        var playerData = players.Select(player => new ListItemData
        {
            label = player.Data["PlayerName"].Value,
            onClick = () => { }
        }).ToList();

        playerList.SetItems(playerData);

        var fungalData = fungalCollection.Fungals.Select(fungal => new ListItemData
        {
            label = fungal.Id,
            sprite = fungal.ActionImage,
            onClick = () => { }
        }).ToList();

        fungalList.SetItems(fungalData);

        if (!joining && multiplayer.JoinedLobby.Data.ContainsKey("JoinCode") && !string.IsNullOrEmpty(multiplayer.JoinedLobby.Data["JoinCode"].Value))
        {
            Debug.Log("joining");
            sceneNavigation.NavigateToScene(1);
            joining = true;
        }
    }

    private bool joining = false;

    private void MultiplayerManager_OnLobbyJoined()
    {
        startButton.gameObject.SetActive(multiplayer.IsHost);
    }
}
