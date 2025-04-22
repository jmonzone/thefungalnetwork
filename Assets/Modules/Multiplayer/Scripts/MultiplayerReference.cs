using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Collections;
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

[Serializable]
public class LobbyPlayer
{
    [SerializeField] public string lobbyId { get; private set; }
    [SerializeField] public int fungal { get; private set; }
    [SerializeField] public string name { get; private set; }
    [SerializeField] public bool isAI { get; private set; }
    [SerializeField] public bool isHost { get; private set; }

    public LobbyPlayer(string lobbyId, int fungal, string name, bool isAI, bool isHost)
    {
        if (string.IsNullOrWhiteSpace(lobbyId)) throw new ArgumentException("lobbyId is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is required.");

        this.lobbyId = lobbyId;
        this.fungal = fungal;
        this.name = name;
        this.isAI = isAI;
        this.isHost = isHost;
    }
}


[CreateAssetMenu]
public class MultiplayerReference : ScriptableObject
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private FungalCollection fungalCollection;

    private string PlayerName => displayName.Value;
    public bool IsSignedIn { get; private set; }

    private Lobby joinedLobby;
    public Lobby JoinedLobby
    {
        get => joinedLobby;
        private set
        {
            joinedLobby = value;
            UpdateLobbyPlayers();
        }
    }

    private void UpdateLobbyPlayers()
    {
        //Debug.Log("UpdateLobbyPlayers");

        if (joinedLobby != null)
        {
            LobbyPlayers = new List<LobbyPlayer>();
            foreach (var player in joinedLobby.Players)
            {
                var fungal = GetPlayerFungalIndex(player);
                var name = player.Data.TryGetValue("PlayerName", out var playerNameData) ? playerNameData.Value : "Unknown Player";
                var isHost = player.Id == JoinedLobby.HostId;
                var lobbyPlayer = new LobbyPlayer(player.Id, fungal, name, false, isHost);

                LobbyPlayers.Add(lobbyPlayer);
            }

            List<LobbyPlayer> botPlayers = GetBotPlayers();

            LobbyPlayers.AddRange(botPlayers);
        }

        OnLobbyUpdated?.Invoke();
    }

    private int GetPlayerFungalIndex(Player player)
    {
        if (player.Data.TryGetValue("Fungal", out var fungalData) && int.TryParse(fungalData?.Value, out var index))
        {
            return Mathf.Clamp(index, 0, fungalCollection.Fungals.Count - 1);
        }
        return 0;
    }

    public List<LobbyPlayer> LobbyPlayers { get; private set; } = new List<LobbyPlayer>();

    public event UnityAction OnLobbyJoined;
    public event UnityAction OnLobbyUpdated;

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

    public async Task CreateRelayAndLobby(GameMode gameMode, int fungalIndex)
    {
        var joinCode = await CreateRelay();

        var player = CreatePlayer(fungalIndex);
        await CreateLobby(player, joinCode, gameMode);

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

            //Debug.Log($"Player data updated: {key} = {value}");
            OnLobbyUpdated?.Invoke();
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

    public async Task JoinRelay(string joinCode)
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


    private Player player;

    private Player CreatePlayer(int fungalIndex = -1) => new Player
    {
        Data = new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) },
            { "Fungal",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, (fungalIndex == -1 ? PlayerPrefs.GetInt("FungalIndex", 0) : fungalIndex).ToString()) }
        }
    };

    public async Task CreateLobby(Player player, string joinCode, GameMode gameMode)
    {
        try
        {
            this.player = player;
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
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString())},
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

    public async Task UpdateJoinedLobbyData(string key, string value)
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
                { key, new DataObject(DataObject.VisibilityOptions.Public, value) }
            }
            };

            Lobby updatedLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, updates);
            hostLobby = updatedLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public string GetJoinedLobbyData(string key)
    {
        //Debug.Log($"GetJoinedLobbyData {key}");

        if (joinedLobby.Data.TryGetValue(key, out var gameModeData))
        {
            //Debug.Log($"GetJoinedLobbyData {key} {gameModeData.Value}");
            return gameModeData.Value;
        }

        return default;
    }

    public GameMode GameMode
    {
        get
        {
            if (Enum.TryParse(GetJoinedLobbyData("GameMode"), out GameMode gameMode)) return gameMode;
            else return default;
        }
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
                    { "HostName", new DataObject(DataObject.VisibilityOptions.Public, PlayerName) },
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, GameMode.PARTY.ToString())},
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
                //Debug.Log($"Lobby: {lobby.Id}, Players: {lobby.Players.Count}/{lobby.MaxPlayers}");
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
            JoinedLobby = null;
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
            Debug.Log("RemoveRelayFromLobbyData");
            await RemoveRelayFromLobbyData();
            //Debug.Log("ResetAIPlayers");
            //await ResetAIPlayers();
        }

        Debug.Log("DisconnectLocalClient");
        NetworkManager.Singleton.GetComponent<UnityTransport>().DisconnectLocalClient();
        Debug.Log("Shutdown");
        NetworkManager.Singleton.Shutdown();
        Debug.Log("NavigateToScene");
        sceneNavigation.NavigateToScene(0);

        if (IsHost)
        {
            Debug.Log("ToggleLobbyLock");
            await ToggleLobbyLock(false);
        }

    }

    public async Task AddAIPlayer(int fungalIndex = -1)
    {
        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to add AI player to.");
            return;
        }

        var aiPlayerNames = GetBotPlayers();
        if (aiPlayerNames.Count >= 8)
        {
            Debug.Log("AI player limit reached.");
            return;
        }

        string newAIName = GenerateUniqueAIName(aiPlayerNames);
        if (string.IsNullOrEmpty(newAIName))
        {
            Debug.LogWarning("Failed to generate unique AI name.");
            return;
        }

        var lobbyId = Guid.NewGuid().ToString("N");
        var fungal = fungalIndex == -1 ? GetAvailableFungalIndex() : fungalIndex;
        var lobbyPlayer = new LobbyPlayer(lobbyId, fungal, newAIName, true, false);

        aiPlayerNames.Add(lobbyPlayer);

        await UpdateLobbyAIData(aiPlayerNames);
        //Debug.Log($"AI player added: {newAIName}");
        UpdateLobbyPlayers();
    }

    private string GenerateUniqueAIName(List<LobbyPlayer> existingPlayers, int maxAttempts = 20)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            string newName = GenerateRandomName();
            if (!existingPlayers.Any(player => player.name == newName))
                return newName;
        }
        return null; // Failed to generate unique name
    }

    private int GetAvailableFungalIndex()
    {
        var usedByHumans = new HashSet<int>(LobbyPlayers.Where(p => !p.isAI).Select(p => p.fungal));
        var usedByAI = new HashSet<int>(LobbyPlayers.Where(p => p.isAI).Select(p => p.fungal));

        // Prefer completely unused fungals
        var allIndices = Enumerable.Range(0, fungalCollection.Fungals.Count);
        var unusedFungals = allIndices.Except(usedByHumans).Except(usedByAI).ToList();
        if (unusedFungals.Any()) return unusedFungals.PopRandom();

        // Next, prefer fungals not used by humans (AI can share)
        var availableForAI = allIndices.Except(usedByHumans).ToList();
        if (availableForAI.Any()) return availableForAI.PopRandom();

        // Finally, pick any available (avoiding human ones)
        return allIndices.Except(usedByHumans).FirstOrDefault();
    }

    private int index = 0;
    private string GenerateRandomName()
    {
        string[] nouns = { "gpt", "gmo", "gmw", "nlp", "hbd", "gtg", "omw", "brb" };
        string noun = nouns[index % nouns.Length];
        index++;
        return $"fungal {noun}";
    }

    public async Task RemoveAIPlayer(LobbyPlayer botName)
    {
        //Debug.Log("RemoveAIPlayer " + botName);

        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to remove AI player from.");
            return;
        }

        List<LobbyPlayer> botPlayers = GetBotPlayers();

        var index = botPlayers.FindIndex(player => player.name == botName.name.ToString());

        // Remove the AI player if it exists
        if (index >= 0)
        {
            botPlayers.RemoveAt(index);

            // Update the lobby data
            await UpdateLobbyAIData(botPlayers);

            //Debug.Log($"AI Player '{botName}' removed.");
        }
        else
        {
            Debug.LogWarning($"AI Player '{botName}' not found in the list.");
        }

        UpdateLobbyPlayers();

    }

    public async Task ResetAIPlayers()
    {
        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to reset AI players.");
            return;
        }

        // Clear the AI player list
        List<LobbyPlayer> botPlayers = new List<LobbyPlayer>();

        // Update the lobby data with an empty AI list
        await UpdateLobbyAIData(botPlayers);

        Debug.Log("All AI players have been removed.");
    }

    public List<LobbyPlayer> GetBotPlayers()
    {
        if (JoinedLobby == null)
        {
            Debug.LogWarning("No joined lobby to get AI players from.");
            return new List<LobbyPlayer>();
        }

        if (JoinedLobby.Data.TryGetValue("BotNames", out var botNamesData))
        {
            try
            {
                return JsonConvert.DeserializeObject<List<LobbyPlayer>>(botNamesData.Value) ?? new List<LobbyPlayer>();
            }
            catch
            {
                Debug.LogError("Failed to deserialize BotNames data.");
                return new List<LobbyPlayer>();
            }
        }

        return new List<LobbyPlayer>();
    }


    private async Task UpdateLobbyAIData(List<LobbyPlayer> aiPlayerNames)
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

            //Debug.Log("Lobby AI player data updated successfully.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to update lobby AI player data: {e.Message}");
        }
    }

    //public List<string> GetAllPlayerNames()
    //{
    //    var playerNames = new List<>();

    //    if (JoinedLobby == null)
    //    {
    //        Debug.LogWarning("No joined lobby.");
    //        return playerNames;
    //    }

    //    // 1. Add human player names
    //    foreach (var player in JoinedLobby.Players)
    //    {
    //        string name = player.Data != null && player.Data.TryGetValue("PlayerName", out var displayName)
    //            ? displayName.Value
    //            : "Unnamed Player";

    //        playerNames.Add(name);
    //    }

    //    // 2. Add AI player names
    //    var aiPlayerNames = GetCurrentAIPlayerList();
    //    playerNames.AddRange(aiPlayerNames);

    //    return playerNames;
    //}

}
