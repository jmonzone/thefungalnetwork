using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuParty : MonoBehaviour
{
    [Header("Navigation & Managers")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private FungalCollection fungalCollection;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI gameModeTitle;
    [SerializeField] private TextMeshProUGUI gameModeDescription;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button gameModeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject waitingIndicator;

    [Header("Selected Fungal")]
    [SerializeField] private TextMeshProUGUI fungalNameText;
    [SerializeField] private TextMeshProUGUI fungalDescriptionText;
    [SerializeField] private Transform fungalSpawnAnchor;
    private List<GameObject> fungalObjects = new List<GameObject>();

    [Header("Fungal Selection")]
    [SerializeField] private Transform fungalSelectionItemAnchor;
    [SerializeField] private FungalSelectionItem fungalSelectionItemPrefab;
    private List<FungalSelectionItem> fungalSelectionItems = new List<FungalSelectionItem>();
    private int selectedFungalIndex = 0;

    [Header("Player List")]
    [SerializeField] private Transform playerListItemAnchor;
    [SerializeField] private PlayerListItem playerListItemPrefab;
    [SerializeField] private Button addBotButton;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();

    private bool joining = false;

    private void Awake()
    {
        InitializeButtons();
        InitializePlayerList();
        InitializeFungals();
        RegisterViewControllerEvents();

        var savedIndex = PlayerPrefs.GetInt("FungalIndex", 0);
        SetSelectedFungalIndex(savedIndex);
    }

    private void OnEnable()
    {
        multiplayer.OnLobbyUpdated += OnMultiplayerLobbyUpdated;
        multiplayer.OnLobbyJoined += OnHostChanged;
        OnHostChanged();
    }

    private void OnDisable()
    {
        multiplayer.OnLobbyUpdated -= OnMultiplayerLobbyUpdated;
        multiplayer.OnLobbyJoined -= OnHostChanged;
    }

    private void InitializeButtons()
    {
        startButton.onClick.AddListener(async () =>
        {
            startButton.interactable = false;
            await multiplayer.ToggleLobbyLock(true);
            var joinCode = await multiplayer.CreateRelay();
            await multiplayer.AddRelayToLobby(joinCode);
        });

        exitButton.onClick.AddListener(async () =>
        {
            await multiplayer.LeaveLobby();
            navigation.GoBack();
        });

        gameModeButton.onClick.AddListener(() =>
        {
            gameModeButton.interactable = false;
            CycleGameMode();
            gameModeButton.interactable = true;
        });

        addBotButton.onClick.AddListener(async () =>
        {
            //Debug.Log("addBotButton click");
            addBotButton.interactable = false;
            await multiplayer.AddAIPlayer();
            addBotButton.interactable = true;
        });
    }

    private void InitializePlayerList()
    {
        playerListItemAnchor.GetComponentsInChildren(includeInactive: true, playerListItems);
        foreach (var item in playerListItems)
        {
            item.Reset();
        }
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

    private void OnMultiplayerLobbyUpdated()
    {
        if (multiplayer.JoinedLobby == null) return;

        UpdatePlayerList();
        HandleLobbyJoin();
        OnHostChanged();
        OnGameModeChanged();
    }

    private void UpdatePlayerList()
    {
        //Debug.Log("UpdatePlayerList " + multiplayer.LobbyPlayers.Count);
        var i = 0;
        foreach (var item in playerListItems)
        {
            if (i < multiplayer.LobbyPlayers.Count)
            {
                var player = multiplayer.LobbyPlayers[i];
                item.SetPlayer(player);
            }
            else
            {
                item.Reset();
            }

            i++;
        }

        addBotButton.gameObject.SetActive(multiplayer.LobbyPlayers.Count < 8);
    }

    private async System.Threading.Tasks.Task SelectFungal(int index)
    {
        fungalObjects[selectedFungalIndex].SetActive(false);
        fungalSelectionItems[selectedFungalIndex].SetIsSelected(false);

        SetSelectedFungalIndex(index);

        fungalObjects[selectedFungalIndex].SetActive(true);

        OnMultiplayerLobbyUpdated();
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
        gameModeButton.gameObject.SetActive(isHost);
        addBotButton.gameObject.SetActive(isHost);
        startButton.gameObject.SetActive(isHost);
        waitingIndicator.SetActive(!isHost);
    }

    private void OnGameModeChanged()
    {
        switch (multiplayer.GameMode)
        {
            case GameMode.PARTY:
                gameModeTitle.text = "Free for All";
                gameModeDescription.text = "999, it's a good slime";
                break;
            case GameMode.ELIMINATION:
                gameModeTitle.text = "Elimination";
                gameModeDescription.text = "Be the last Fungal standing";
                break;
        }
    }
}
