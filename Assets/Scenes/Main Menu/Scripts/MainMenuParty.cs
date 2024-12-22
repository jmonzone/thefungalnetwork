using System;
using System.Linq;
using GURU;
using Unity.Services.Authentication;
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
            sceneNavigation.NavigateToScene(1);
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

    private int localPlayerFungalIndex = 0;

    private void MultiplayerManager_OnLobbyPoll()
    {

        var playerData = multiplayer.JoinedLobby.Players.Select(player =>
        {
            bool isLocalPlayer = player.Id == AuthenticationService.Instance.PlayerId;

            int fungalIndex = 0;

            if (isLocalPlayer) fungalIndex = localPlayerFungalIndex;
            else
            {
                if (player.Data.TryGetValue("Fungal", out var fungalData))
                {
                    if (int.TryParse(fungalData?.Value, out var index))
                    {
                        fungalIndex = index;
                    }
                }
            }

            fungalIndex = Math.Clamp(fungalIndex, 0, fungalCollection.Fungals.Count - 1);

            var targetFungal = fungalCollection.Fungals[fungalIndex];

            return new ListItemData
            {
                label = player.Data.TryGetValue("PlayerName", out var playerNameData)
                        ? playerNameData.Value
                        : "Unknown Player",
                sprite = targetFungal.ActionImage,
                onClick = () => { }
            };
        }).ToList();

        playerList.SetItems(playerData);

        var fungalData = fungalCollection.Fungals.Select((fungal, index) => new ListItemData
        {
            label = fungal.Id,
            sprite = fungal.ActionImage,
            onClick = async () =>
            {
                localPlayerFungalIndex = index;
                MultiplayerManager_OnLobbyPoll();
                await multiplayer.AddPlayerDataToLobby("Fungal", index.ToString());
            }
        }).ToList();

        fungalList.SetItems(fungalData);

        if (!joining && multiplayer.JoinedLobby.Data.ContainsKey("JoinCode") && !string.IsNullOrEmpty(multiplayer.JoinedLobby.Data["JoinCode"].Value))
        {
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
