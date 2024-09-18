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

public class MultiplayerManager : MonoBehaviour
{
    public string PlayerName { get; private set; }


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

    private int maxPlayers = 10;

    private Lobby hostLobby;

    private bool joinedRelay;


    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    private void Update()
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

            if (!joinedRelay)
            {
                JoinRelay(JoinedLobby.Data["JoinCode"].Value);
                joinedRelay = true;
            }
        }
    }

    public async void SignIn(string playerName, UnityAction onComplete)
    {
        PlayerName = playerName.Replace(" ", "_");
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(PlayerName);

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => onComplete?.Invoke();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task CreateRelayAndLobby()
    {
        var joinCode = await CreateRelay();
        await CreateLobby(joinCode);

    }

    private async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.OnClientDisconnectCallback += async (clientId) =>
            {
                Debug.Log($"Client disconnected: {clientId}");

                foreach(var player in joinedLobby.Players)
                {
                    Debug.Log($"player: {player.Data["NetworkId"].Value}");

                    if (player.Data["NetworkId"].Value == clientId.ToString())
                    {
                        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, player.Id);
                    }
                }


            };

            joinedRelay = true;

            return joinCode;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }

    }

    private async void JoinRelay(string joinCode)
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
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }


    private async Task CreateLobby(string joinCode)
    {
        try
        {
            var player = Player;
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

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("Test Lobby", maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            JoinedLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = Player,
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            JoinedLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = Player,
            };

            // Join the lobby using its ID
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            JoinedLobby = lobby;

            // Confirm that the lobby has been joined
            Debug.Log($"Successfully joined lobby with ID: {lobby.Id}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby by ID: {e}");
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
            Debug.Log("Available Lobbies:");
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

    private Player Player => new Player
    {
        Data = new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) }

        }
    };
}
