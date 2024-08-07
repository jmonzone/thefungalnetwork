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

        AuthenticationService.Instance.SignedIn += () => onComplete();

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

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
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
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = Player,
                Data = new Dictionary<string, DataObject>()
                {
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
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

    private async Task QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = Player,
            };

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
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

    private Player Player => new Player
    {
        Data = new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) }
        }
    };

    private async void MigrateHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = JoinedLobby.Players[1].Id
            });

            JoinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void OnApplicationQuit()
    {
        MigrateHost();
    }
}
