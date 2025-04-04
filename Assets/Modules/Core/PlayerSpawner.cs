using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private NetworkFungal fungalPrefab;

    public event Action<PlayerSpawner> OnSpawnerReady;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            //var selectedFungals = new HashSet<int>();

            //// Collect already selected fungal indices
            //foreach (var player in multiplayer.JoinedLobby.Players)
            //    if (player.Data.TryGetValue("Fungal", out var fungalData) &&
            //        int.TryParse(fungalData?.Value, out var index))
            //        selectedFungals.Add(index);

            //// Get available indices
            //var availableFungals = Enumerable.Range(0, fungalCollection.Fungals.Count)
            //                                 .Where(i => !selectedFungals.Contains(i))
            //                                 .ToList();

            //int i = 0;
            //foreach (var aiPlayer in multiplayer.GetCurrentAIPlayerList())
            //{
            //    ulong aiClientId = NetworkManager.Singleton.LocalClientId;
            //    int fungalIndex = availableFungals.Any() ? availableFungals.PopRandom() : UnityEngine.Random.Range(0, fungalCollection.Fungals.Count);

            //    AddPlayer(aiClientId, aiPlayer, i + multiplayer.JoinedLobby.Players.Count, fungalIndex, isAI: true);
            //    i++;
            //}

            OnSpawnerReady?.Invoke(this);
        }
    }


    public void AddPlayer(ulong clientId, string playerName, int playerIndex, int fungalIndex, bool isAI = false)
    {
        //Debug.Log("AddPlayer");
        OnPlayerAddedServerRpc(clientId, playerName, playerIndex, fungalIndex, isAI);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerAddedServerRpc(ulong clientId, FixedString64Bytes playerName, int playerIndex, int fungalIndex, bool isAI = false)
    {
        //Debug.Log("OnPlayerAddedServerRpc");
       
        var spawnOrigin = arena.SpawnPositions[playerIndex].position;

        var spawnPosition = spawnOrigin;

        var networkFungal = Instantiate(fungalPrefab, spawnPosition, Quaternion.identity);
        networkFungal.NetworkObject.SpawnWithOwnership(clientId);
        networkFungal.InitializeServerRpc(playerName, playerIndex, fungalIndex, isAI);

        if (!isAI)
        {
            OnPlayerAddedClientRpc(clientId, playerIndex, networkFungal.NetworkObjectId);
        }
    }

    [ClientRpc]
    private void OnPlayerAddedClientRpc(ulong clientId, int playerIndex, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            var networkFungal = networkObject.GetComponent<NetworkFungal>();
            pufferballReference.AddPlayer(clientId, playerIndex, networkFungal);
        }

        //Debug.Log("Client owner spawned, searching for existing NetworkFungals...");

        // Find all existing NetworkFungal objects using the SpawnManager
        foreach (var spawnedObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            var networkFungal = spawnedObject.GetComponent<NetworkFungal>();
            if (networkFungal != null)
            {
                // Check if a player with the same index already exists
                if (pufferballReference.Players.Any(player => player.Fungal == networkFungal))
                {
                    continue;
                }

                // Register the fungal with the pufferballReference
                pufferballReference.AddPlayer(clientId, networkFungal.Index, networkFungal);
            }
        }
    }

    private void OnEnable()
    {
        multiplayer.OnDisconnectRequested += NotifyClientsDisconnectServerRpc;
    }

    private void OnDisable()
    {
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
