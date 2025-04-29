using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

[Serializable]
public struct LobbyPlayerRPCParam : INetworkSerializable
{
    public FixedString64Bytes lobbyId;
    public int fungal;
    public FixedString64Bytes name;
    public bool isAI;
    public bool isHost;

    // Constructor from LobbyPlayer
    public LobbyPlayerRPCParam(LobbyPlayer player)
    {
        lobbyId = new FixedString64Bytes(player.lobbyId);
        fungal = player.fungal;
        name = new FixedString64Bytes(player.name);
        isAI = player.isAI;
        isHost = player.isHost;
    }

    // Convert back to LobbyPlayer
    public LobbyPlayer ToLobbyPlayer()
    {
        return new LobbyPlayer(lobbyId.ToString(), fungal, name.ToString(), isAI, isHost);
    }

    // Netcode serialization
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref lobbyId);
        serializer.SerializeValue(ref fungal);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref isAI);
        serializer.SerializeValue(ref isHost);
    }
}

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private GameReference game;
    [SerializeField] private NetworkFungal fungalPrefab;

    [Header("Power Ups")]
    [SerializeField] private List<PowerUp> powerUps;
    [SerializeField] private List<Ability> abilities;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            int i = 0;
            foreach (var bot in multiplayer.GetBotPlayers())
            {
                // todo: consider adding botClientId to botPlayer obj
                ulong botClientId = NetworkManager.Singleton.LocalClientId;
                var rpcPlayer = new LobbyPlayerRPCParam(bot);
                Debug.Log($"PlayerSpawner {bot.fungal} {rpcPlayer.fungal} ");
                AddPlayerServerRpc(botClientId, i + multiplayer.JoinedLobby.Players.Count, rpcPlayer);
                i++;
            }
        }

        for(var i = 0; i < powerUps.Count; i++)
        {
            var ability = abilities[i];
            powerUps[i].AssignAbility(ability);
        }

        AddPlayerToSpawner();
    }

    private void AddPlayerToSpawner()
    {
        string localPlayerId = AuthenticationService.Instance.PlayerId;

        var localPlayerIndex = multiplayer.LobbyPlayers.FindIndex(player => player.lobbyId == localPlayerId);
        var localPlayer = multiplayer.LobbyPlayers[localPlayerIndex];

        var rpcPlayer = new LobbyPlayerRPCParam(localPlayer);

        AddPlayerServerRpc(NetworkManager.Singleton.LocalClientId, localPlayerIndex, rpcPlayer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerServerRpc(ulong clientId, int playerIndex, LobbyPlayerRPCParam player)
    {
        //Debug.Log($"AddPlayerServerRpc {playerIndex} {player.name} {player.fungal}");

        var spawnOrigin = arena.SpawnPositions[playerIndex].position;

        var spawnPosition = spawnOrigin;

        var networkFungal = Instantiate(fungalPrefab, spawnPosition, Quaternion.identity);
        networkFungal.NetworkObject.SpawnWithOwnership(clientId);
        networkFungal.InitializeServerRpc(playerIndex, player);

        if (!player.isAI)
        {
            OnPlayerAddedClientRpc(clientId, playerIndex, player, networkFungal.NetworkObjectId);
        }
    }

    [ClientRpc]
    private void OnPlayerAddedClientRpc(ulong clientId, int playerIndex, LobbyPlayerRPCParam player, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            var networkFungal = networkObject.GetComponent<NetworkFungal>();
            game.AddPlayer(clientId, player.name, playerIndex, networkFungal);
        }

        //Debug.Log("Client owner spawned, searching for existing NetworkFungals...");

        // Find all existing NetworkFungal objects using the SpawnManager
        foreach (var spawnedObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            var networkFungal = spawnedObject.GetComponent<NetworkFungal>();
            if (networkFungal != null)
            {
                // Check if a player with the same index already exists
                if (game.Players.Any(player => player.Fungal == networkFungal))
                {
                    continue;
                }

                // Register the fungal with the pufferballReference
                game.AddPlayer(networkFungal.OwnerClientId, networkFungal.PlayerName, networkFungal.Index, networkFungal);
            }
        }
    }


    // todo: consider separating disconnect logic
    // todo: player spawnere logic can be a part of network prefab?
    private void OnEnable()
    {
        game.OnGameStart += Game_OnGameStart;
        game.OnGameComplete += Game_OnGameComplete;
        multiplayer.OnDisconnectRequested += NotifyClientsDisconnectServerRpc;
    }

    private void Game_OnGameComplete()
    {
        game.Players.ForEach(player =>
        {
            if (player.Fungal.Fungal.IsBot)
            {
                player.Fungal.GetComponent<FungalAI>().StopAI();
            }
        });
    }

    private void Game_OnGameStart()
    {
        game.Players.ForEach(player =>
        {
            if (player.Fungal.Fungal.IsBot)
            {
                player.Fungal.GetComponent<FungalAI>().StartAI();
            }
        });
    }

    private void OnDisable()
    {
        game.OnGameStart -= Game_OnGameStart;
        game.OnGameComplete -= Game_OnGameComplete;
        multiplayer.OnDisconnectRequested -= NotifyClientsDisconnectServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyClientsDisconnectServerRpc()
    {
        Debug.Log("AutoConnect NotifyClientsDisconnectServerRpc");
        NotifyClientsDisconnectClientRpc();
    }

    [ClientRpc]
    private void NotifyClientsDisconnectClientRpc()
    {
        Debug.Log("AutoConnect NotifyClientsDisconnectClientRpc");
        multiplayer.DisconnectFromRelay();
    }
}
