using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GURU;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuParty : MonoBehaviour
{
    [SerializeField] private ListUI playerList;
    [SerializeField] private ListUI fungalList;
    [SerializeField] private GameObject waitingIndicator;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private Navigation navigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private MultiplayerManager multiplayer;

    [SerializeField] private Transform fungalSpawnAnchor;

    [SerializeField] private TextMeshProUGUI gameModeTitle;
    [SerializeField] private TextMeshProUGUI gameModeDescription;
    [SerializeField] private GameObject hostControls;
    [SerializeField] private Button gameModeButton;

    [SerializeField] private Button useAiPlayersButton;
    [SerializeField] private TextMeshProUGUI useAiPlayersText;

    private Dictionary<int, GameObject> fungalObjects = new Dictionary<int, GameObject>();
    private int selectedFungalIndex = 0;

    private GameMode gameMode;
    private bool useAIPlayers = true;

    private void Awake()
    {
        startButton.onClick.AddListener(async () =>
        {
            await multiplayer.ToggleLobbyLock(true);
            var joinCode = await multiplayer.CreateRelay();
            await multiplayer.AddRelayToLobby(joinCode);
        });

        exitButton.onClick.AddListener(async () =>
        {
            await multiplayer.LeaveLobby();
            navigation.GoBack();
        });

        gameModeButton.onClick.AddListener(async () =>
        {
            gameModeButton.interactable = false;
            await multiplayer.UpdateJoinedLobbyData("GameMode", (gameMode == GameMode.PARTY ? GameMode.ELIMINATION : GameMode.PARTY).ToString());
            gameModeButton.interactable = true;
        });

        useAiPlayersButton.onClick.AddListener(async () =>
        {
            useAiPlayersButton.interactable = false;
            useAIPlayers = !useAIPlayers;
            useAiPlayersText.text = $"Use AI Players: {(useAIPlayers ? "On" : "Off")}";
            await multiplayer.UpdateJoinedLobbyData("UseAI", useAIPlayers.ToString());
            useAiPlayersButton.interactable = true;
        });

        useAiPlayersText.text = $"Use AI Players: {(useAIPlayers ? "On" : "Off")}";

        for (var i = 0; i < fungalCollection.Fungals.Count; i++)
        {
            var fungal = fungalCollection.Fungals[i];
            var prefab = Instantiate(fungal.Prefab, fungalSpawnAnchor);
            prefab.transform.SetPositionAndRotation(fungalSpawnAnchor.position, fungalSpawnAnchor.rotation);
            prefab.SetActive(false);
            fungalObjects.Add(i, prefab);
        }

        selectedFungalIndex = PlayerPrefs.GetInt("FungalIndex", 0);

        GetComponent<ViewController>().OnFadeInComplete += () =>
        {
            fungalObjects[selectedFungalIndex].SetActive(true);
        };

        GetComponent<ViewController>().OnFadeOutComplete += () =>
        {
            fungalObjects[selectedFungalIndex].SetActive(false);
        };
    }

    private void OnEnable()
    {
        OnHostChanged();
        multiplayer.OnLobbyPoll += MultiplayerManager_OnLobbyPoll;
        multiplayer.OnLobbyJoined += OnHostChanged;
    }

    private void OnDisable()
    {
        multiplayer.OnLobbyPoll -= MultiplayerManager_OnLobbyPoll;
        multiplayer.OnLobbyJoined -= OnHostChanged;
    }

    private void MultiplayerManager_OnLobbyPoll()
    {
        var playerData = multiplayer.JoinedLobby.Players.Select(player =>
        {
            bool isLocalPlayer = player.Id == AuthenticationService.Instance.PlayerId;

            int fungalIndex = 0;

            if (isLocalPlayer) fungalIndex = selectedFungalIndex;
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
                onClick = () => { },
                showBadge = multiplayer.JoinedLobby.HostId == player.Id
            };
        }).ToList();

        playerList.SetItems(playerData);

        var fungalData = fungalCollection.Fungals.Select((fungal, index) => new ListItemData
        {
            label = fungal.Id,
            sprite = fungal.ActionImage,
            onClick = async () =>
            {
                fungalObjects[selectedFungalIndex].SetActive(false);
                selectedFungalIndex = index;
                PlayerPrefs.SetInt("FungalIndex", selectedFungalIndex); // Default to 0 if key doesn't exist
                fungalObjects[selectedFungalIndex].SetActive(true);

                MultiplayerManager_OnLobbyPoll();
                await multiplayer.AddPlayerDataToLobby("Fungal", index.ToString());
            }
        }).ToList();

        fungalList.SetItems(fungalData);

        if (!joining && multiplayer.JoinedLobby.Data.ContainsKey("JoinCode") && !string.IsNullOrEmpty(multiplayer.JoinedLobby.Data["JoinCode"].Value))
        {
            if (!multiplayer.IsHost)
            {
                var joinCode = multiplayer.JoinedLobby.Data["JoinCode"].Value;
                multiplayer.JoinRelay(joinCode);
            }
            sceneNavigation.NavigateToScene(1);
            joining = true;
        }

        OnHostChanged();
        OnGameModeChanged();
    }

    private bool joining = false;

    private void OnHostChanged()
    {
        hostControls.SetActive(multiplayer.IsHost);

        startButton.gameObject.SetActive(multiplayer.IsHost);
        waitingIndicator.SetActive(!multiplayer.IsHost);
    }

    private void OnGameModeChanged()
    {
        switch (multiplayer.GameMode)
        {
            case GameMode.PARTY:
                gameModeTitle.text = "Party";
                gameModeDescription.text = "999, it's a good slime";
                break;
            case GameMode.ELIMINATION:
                gameModeTitle.text = "Elimination";
                gameModeDescription.text = "Be the last Fungal standing";
                break;
        }
    }
}
