using System.Collections.Generic;
using System.Threading.Tasks;
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

[CreateAssetMenu]
public class MultiplayerManager : ScriptableObject
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private SceneNavigation sceneNavigation;

    public string PlayerName { get; private set; }
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
            PlayerName = displayName.Value.Replace(" ", "_");

            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(PlayerName);

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
            NetworkManager.Singleton.StartHost();

            JoinedRelay = true;

            return joinCode;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }

    }

    public async Task AddRelayToLobby(string joinCode)
    {
        try
        {
            var data = JoinedLobby.Data;
            data["JoinCode"] = new DataObject(DataObject.VisibilityOptions.Member, joinCode);
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = data,
                IsLocked = true
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

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();

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
                    { "HostName", new DataObject(DataObject.VisibilityOptions.Public, PlayerName)},
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
        if (IsHost) await RemoveRelayFromLobbyData();

        NetworkManager.Singleton.GetComponent<UnityTransport>().DisconnectLocalClient();
        NetworkManager.Singleton.Shutdown();
        sceneNavigation.NavigateToScene(0);
    }

    private Player player;

    private Player CreatePlayer() => new Player
    {
        Data = new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) },
            { "Fungal",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerPrefs.GetInt("FungalIndex", 0).ToString()) }
        }
    };
}
