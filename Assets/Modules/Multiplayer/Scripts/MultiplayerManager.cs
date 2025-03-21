using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;

public enum GameMode
{
    PARTY,
    ELIMINATION
}

[CreateAssetMenu]
public class MultiplayerManager : ScriptableObject
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private SceneNavigation sceneNavigation;

    public string PlayerName => displayName.Value ?? "Unknown";
    public bool IsSignedIn { get; private set; }

    private Lobby joinedLobby;
    public Lobby JoinedLobby
    {
        get => joinedLobby;
        private set
        {
            joinedLobby = value;
            OnLobbyUpdated?.Invoke();
        }
    }

    public event UnityAction OnLobbyUpdated;
    public event UnityAction OnLobbyJoined;
    public event UnityAction OnLobbyPoll;

    private const int MAX_PLAYER_COUNT = 4;

    private Lobby hostLobby;

    public bool JoinedRelay { get; private set; }

    public bool IsHost => JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;

    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    public void Initialize()
    {
        JoinedLobby = null;
        IsSignedIn = false;
    }

    public void DoUpdate()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby == null) return;

        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer < 0f)
        {
            heartbeatTimer = 15f;
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }


    private async void HandleLobbyPollForUpdates()
    {
        if (JoinedLobby == null) return;

        lobbyUpdateTimer -= Time.deltaTime;
        if (lobbyUpdateTimer < 0f)
        {
            lobbyUpdateTimer = 1.1f;
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
            JoinedLobby = lobby;
            if (IsHost) hostLobby = lobby;
            else hostLobby = null;
            OnLobbyPoll?.Invoke();
        }
    }

    public async void SignIn(UnityAction onComplete)
    {
        if (IsSignedIn)
        {
            onComplete?.Invoke();
        }
        else
        {
            await UnityServices.InitializeAsync();

            InitializationOptions initializationOptions = new InitializationOptions();

            // Example player name input
            string playerNameInput = "Some@Player!Name#123_ExtraStuff";

            // Sanitize: Keep only alphanumeric, '-', '_'
            string sanitizedPlayerName = Regex.Replace(playerNameInput, @"[^a-zA-Z0-9\-_]", "");

            // Trim to a max length of 30 characters
            if (sanitizedPlayerName.Length > 30)
            {
                sanitizedPlayerName = sanitizedPlayerName.Substring(0, 30);
            }

            // Optional: Fallback if name ends up empty after sanitizing
            if (string.IsNullOrEmpty(sanitizedPlayerName))
            {
                sanitizedPlayerName = "DefaultPlayer";
            }

            // Set the profile
            initializationOptions.SetProfile(sanitizedPlayerName);
            AuthenticationService.Instance.SignedIn += () =>
            {
                IsSignedIn = true;
                onComplete?.Invoke();
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
    }

    public async Task CreateRelayAndLobby()
    {
        var joinCode = await CreateRelay();
        await CreateLobby(joinCode);

    }

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER_COUNT - 1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            JoinedRelay = true;

            return joinCode;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }

    }

    public async Task AddRelayToLobby(string relayCode)
    {
        try
        {
            var data = JoinedLobby.Data;
            data["JoinCode"] = new DataObject(DataObject.VisibilityOptions.Member, relayCode);
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = data,
            });

            JoinedLobby = lobby;
            OnLobbyPoll?.Invoke();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task ToggleLobbyLock(bool value)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                IsLocked = value,
            });

            JoinedLobby = lobby;
            OnLobbyPoll?.Invoke();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task AddPlayerDataToLobby(string key, string value)
    {
        try
        {
            var playerData = new Dictionary<string, PlayerDataObject>
            {
                { key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value) }
            };

            await LobbyService.Instance.UpdatePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = playerData
            });

            Debug.Log($"Player data updated: {key} = {value}");
            OnLobbyPoll?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to update player data: {e}");
        }
    }


    public async Task RemoveRelayFromLobbyData()
    {
        try
        {
            var data = JoinedLobby.Data;
            data["JoinCode"] = new DataObject(DataObject.VisibilityOptions.Member, null);
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = data
            });

            JoinedLobby = lobby;
            OnLobbyPoll?.Invoke();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void StartHostClient()
    {
        if (IsHost) NetworkManager.Singleton.StartHost();
        else NetworkManager.Singleton.StartClient();
    }



    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.OnClientConnectedCallback += async (clientId) =>
            {
                await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                {
                    { "NetworkId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, NetworkManager.Singleton.LocalClientId.ToString()) }
                }
                });
            };

            JoinedRelay = true;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }


    private Unity.Services.Lobbies.Models.Player player;

    private Unity.Services.Lobbies.Models.Player CreatePlayer() => new Unity.Services.Lobbies.Models.Player
    {
        Data = new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) },
            { "Fungal",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerPrefs.GetInt("FungalIndex", 0).ToString()) }
        }
    };

    public async Task CreateLobby(string joinCode)
    {
        try
        {
            player = CreatePlayer();
            var clientId = NetworkManager.Singleton.LocalClientId;
            player.Data["NetworkId"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, clientId.ToString());

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>()
            {
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) },
                { "HostName", new DataObject(DataObject.VisibilityOptions.Public, PlayerName) },
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, GameMode.PARTY.ToString()) }
            }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("Test Lobby", MAX_PLAYER_COUNT, createLobbyOptions);
            hostLobby = lobby;
            JoinedLobby = lobby;

            OnLobbyJoined?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task UpdateGameMode(GameMode newGameMode)
    {
        if (hostLobby == null)
        {
            Debug.LogWarning("No lobby to update!");
            return;
        }

        try
        {
            var updates = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>()
            {
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, newGameMode.ToString()) }
            }
            };

            Lobby updatedLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, updates);
            hostLobby = updatedLobby;

            Debug.Log("Game mode updated to: " + newGameMode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public GameMode GetGameMode(Lobby lobby)
    {
        if (lobby.Data.TryGetValue("GameMode", out var gameModeData))
        {
            if (Enum.TryParse(gameModeData.Value, out GameMode res))
            {
                return res;
            }
        }

        return default;
    }

    public async Task CreateLobby()
    {
        try
        {
            player = CreatePlayer();
            var clientId = NetworkManager.Singleton.LocalClientId;
            player.Data["NetworkId"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, clientId.ToString());

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>()
                {
                    { "HostName", new DataObject(DataObject.VisibilityOptions.Public, PlayerName)},
                },
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("Test Lobby", MAX_PLAYER_COUNT, createLobbyOptions);
            hostLobby = lobby;
            JoinedLobby = lobby;

            OnLobbyJoined?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<bool> TryJoinLobbyById(string lobbyId)
    {
        try
        {
            player = CreatePlayer();
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = player,
            };

            // Join the lobby using its ID
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            JoinedLobby = lobby;

            OnLobbyJoined?.Invoke();
            // Confirm that the lobby has been joined
            Debug.Log($"Successfully joined lobby with ID: {lobby.Id}");
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby by ID: {e}");
            return false;
        }
    }

    public async void ListLobbies(UnityAction<List<Lobby>> onComplete)
    {
        try
        {
            // Set the query options such as max number of lobbies to return
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 10, // Max number of lobbies to return
                Filters = new List<QueryFilter>
                {
                    // Example filter: only open lobbies
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                }
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            // Display found lobbies
            //Debug.Log("Available Lobbies:");
            foreach (Lobby lobby in lobbies.Results)
            {
                Debug.Log($"Lobby: {lobby.Id}, Players: {lobby.Players.Count}/{lobby.MaxPlayers}");
            }

            onComplete?.Invoke(lobbies.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to list lobbies: {e}");
        }
    }

    public async Task LeaveLobby()
    {
        if (joinedLobby == null) return;

        try
        {
            //var playerId = player.Data["NetworkId"].Value;
            string playerId = AuthenticationService.Instance.PlayerId;

            // Remove the player from the lobby
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            joinedLobby = null;
            player = null;
            Debug.Log("Player removed from lobby");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Error leaving the lobby: {e.Message}");
        }
    }

    public event UnityAction OnDisconnectRequested;

    public void RequestDisconnect()
    {
        OnDisconnectRequested?.Invoke();
    }

    public async void DisconnectFromRelay()
    {
        if (IsHost)
        {
            await RemoveRelayFromLobbyData();
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().DisconnectLocalClient();
        NetworkManager.Singleton.Shutdown();
        sceneNavigation.NavigateToScene(0);

        if (IsHost)
        {
            await ToggleLobbyLock(false);
        }

    }

    /// <summary>
    /// Adds an AI player to the lobby data.
    /// </summary>
    public async Task AddAIPlayer(string aiPlayerName)
    {
        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to add AI player to.");
            return;
        }

        List<string> aiPlayerNames = GetCurrentAIPlayerList();

        // Add the new AI player
        aiPlayerNames.Add(aiPlayerName);

        // Update the lobby data
        await UpdateLobbyAIData(aiPlayerNames);

        Debug.Log($"AI Player '{aiPlayerName}' added.");
    }

    /// <summary>
    /// Removes an AI player from the lobby data.
    /// </summary>
    public async Task RemoveAIPlayer(string aiPlayerName)
    {
        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to remove AI player from.");
            return;
        }

        List<string> aiPlayerNames = GetCurrentAIPlayerList();

        // Remove the AI player if it exists
        if (aiPlayerNames.Remove(aiPlayerName))
        {
            // Update the lobby data
            await UpdateLobbyAIData(aiPlayerNames);

            Debug.Log($"AI Player '{aiPlayerName}' removed.");
        }
        else
        {
            Debug.LogWarning($"AI Player '{aiPlayerName}' not found in the list.");
        }
    }

    /// <summary>
    /// Retrieves the current AI player list from lobby data.
    /// </summary>
    public List<string> GetCurrentAIPlayerList()
    {
        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to get AI players from.");
            return new List<string>();
        }

        // Get the AI player list from the lobby data
        if (JoinedLobby.Data.TryGetValue("BotNames", out var botNamesData))
        {
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(botNamesData.Value) ?? new List<string>();
            }
            catch
            {
                Debug.LogError("Failed to deserialize BotNames data.");
                return new List<string>();
            }
        }

        // No bots yet
        return new List<string>();
    }

    /// <summary>
    /// Updates the lobby data with the new AI player list.
    /// </summary>
    private async Task UpdateLobbyAIData(List<string> aiPlayerNames)
    {
        try
        {
            var updatedData = new Dictionary<string, DataObject>()
            {
                { "BotCount", new DataObject(DataObject.VisibilityOptions.Public, aiPlayerNames.Count.ToString()) },
                { "BotNames", new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(aiPlayerNames)) }
            };

            JoinedLobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = updatedData
            });

            Debug.Log("Lobby AI player data updated successfully.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to update lobby AI player data: {e.Message}");
        }
    }
}
