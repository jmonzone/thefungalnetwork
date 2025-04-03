using System;
using System.Collections.Generic;
using System.Linq;
using GURU;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuParty : MonoBehaviour
{
    [Header("Navigation & Managers")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private FungalCollection fungalCollection;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI gameModeTitle;
    [SerializeField] private TextMeshProUGUI gameModeDescription;
    [SerializeField] private ListUI playerList;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button gameModeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject waitingIndicator;
    [SerializeField] private GameObject hostControls;

    [Header("Fungal Selection")]
    [SerializeField] private Transform fungalSpawnAnchor;
    [SerializeField] private Transform fungalSelectionItemAnchor;
    [SerializeField] private FungalSelectionItem fungalSelectionItemPrefab;
    [SerializeField] private TextMeshProUGUI fungalNameText;
    [SerializeField] private TextMeshProUGUI fungalDescriptionText;

    private List<FungalSelectionItem> fungalSelectionItems = new List<FungalSelectionItem>();
    private List<GameObject> fungalObjects = new List<GameObject>();
    private int selectedFungalIndex = 0;
    private bool joining = false;


    private void Awake()
    {
        InitializeButtons();
        InitializeFungals();
        RegisterViewControllerEvents();

        var savedIndex = PlayerPrefs.GetInt("FungalIndex", 0);
        SetSelectedFungalIndex(savedIndex);
    }

    private void OnEnable()
    {
        multiplayer.OnLobbyPoll += MultiplayerManager_OnLobbyPoll;
        multiplayer.OnLobbyJoined += OnHostChanged;
        OnHostChanged();
    }

    private void OnDisable()
    {
        multiplayer.OnLobbyPoll -= MultiplayerManager_OnLobbyPoll;
        multiplayer.OnLobbyJoined -= OnHostChanged;
    }

    private void InitializeButtons()
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
            CycleGameMode();
            gameModeButton.interactable = true;
        });
    }

    private void InitializeFungals()
    {
        fungalSelectionItemAnchor.GetComponentsInChildren(includeInactive: true, fungalSelectionItems);

        for (var i = fungalSelectionItems.Count; i < fungalCollection.Fungals.Count; i++)
        {
            var item = Instantiate(fungalSelectionItemPrefab, fungalSelectionItemAnchor);
            fungalSelectionItems.Add(item);
        }

        foreach (var item in fungalSelectionItems)
        {
            item.SetIsSelected(false);
            item.gameObject.SetActive(false);
        }

        for (var i = 0; i < fungalCollection.Fungals.Count; i++)
        {
            var fungal = fungalCollection.Fungals[i];
            var prefab = Instantiate(fungal.Prefab, fungalSpawnAnchor);
            prefab.transform.SetPositionAndRotation(fungalSpawnAnchor.position, fungalSpawnAnchor.rotation);
            prefab.SetActive(false);
            fungalObjects.Add(prefab);

            var index = i;
            fungalSelectionItems[i].SetData(fungal, async () => await SelectFungal(index));
        }
    }
    private void RegisterViewControllerEvents()
    {
        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInComplete += () => fungalObjects[selectedFungalIndex].SetActive(true);
        viewController.OnFadeOutComplete += () => fungalObjects[selectedFungalIndex].SetActive(false);
    }

    private async void CycleGameMode()
    {
        GameMode[] modes = (GameMode[])Enum.GetValues(typeof(GameMode));
        int currentIndex = Array.IndexOf(modes, multiplayer.GameMode);
        int nextIndex = (currentIndex + 1) % modes.Length;
        GameMode nextMode = modes[nextIndex];
        await multiplayer.UpdateJoinedLobbyData("GameMode", nextMode.ToString());
    }

    private void MultiplayerManager_OnLobbyPoll()
    {
        UpdatePlayerList();
        HandleLobbyJoin();
        OnHostChanged();
        OnGameModeChanged();
    }

    private void UpdatePlayerList()
    {
        var playerData = multiplayer.JoinedLobby.Players.Select(player =>
        {
            bool isLocalPlayer = player.Id == AuthenticationService.Instance.PlayerId;
            int fungalIndex = isLocalPlayer ? selectedFungalIndex : GetPlayerFungalIndex(player);

            //Debug.Log(fungalIndex);
            var targetFungal = fungalCollection.Fungals[fungalIndex];
            return new ListItemData
            {
                label = player.Data.TryGetValue("PlayerName", out var playerNameData) ? playerNameData.Value : "Unknown Player",
                sprite = targetFungal.ActionImage,
                onClick = () => { },
                showBadge = multiplayer.JoinedLobby.HostId == player.Id
            };

        }).ToList();

        playerList.SetItems(playerData);
    }

    private int GetPlayerFungalIndex(Unity.Services.Lobbies.Models.Player player)
    {
        if (player.Data.TryGetValue("Fungal", out var fungalData) && int.TryParse(fungalData?.Value, out var index))
        {
            return Mathf.Clamp(index, 0, fungalCollection.Fungals.Count - 1);
        }
        return 0;
    }

    private async System.Threading.Tasks.Task SelectFungal(int index)
    {
        fungalObjects[selectedFungalIndex].SetActive(false);
        fungalSelectionItems[selectedFungalIndex].SetIsSelected(false);

        SetSelectedFungalIndex(index);

        fungalObjects[selectedFungalIndex].SetActive(true);

        MultiplayerManager_OnLobbyPoll();
        await multiplayer.AddPlayerDataToLobby("Fungal", index.ToString());
    }

    private void SetSelectedFungalIndex(int index)
    {
        selectedFungalIndex = Mathf.Clamp(index, 0, fungalCollection.Fungals.Count - 1);
        PlayerPrefs.SetInt("FungalIndex", selectedFungalIndex);

        fungalSelectionItems[selectedFungalIndex].SetIsSelected(true);

        var fungal = fungalCollection.Fungals[selectedFungalIndex];
        fungalNameText.text = fungal.name;
        fungalDescriptionText.text = fungal.Description;

    }
    private void HandleLobbyJoin()
    {
        if (!joining && multiplayer.JoinedLobby.Data.ContainsKey("JoinCode") && !string.IsNullOrEmpty(multiplayer.JoinedLobby.Data["JoinCode"].Value))
        {
            if (!multiplayer.IsHost)
            {
                multiplayer.JoinRelay(multiplayer.JoinedLobby.Data["JoinCode"].Value);
            }
            sceneNavigation.NavigateToScene(1);
            joining = true;
        }
    }

    private void OnHostChanged()
    {
        bool isHost = multiplayer.IsHost;
        hostControls.SetActive(isHost);
        startButton.gameObject.SetActive(isHost);
        waitingIndicator.SetActive(!isHost);
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
