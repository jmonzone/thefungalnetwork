using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private bool addAIPlayer = false;
    [SerializeField] private NetworkFungal fungalPrefab;

    public event Action<PlayerSpawner> OnSpawnerReady;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            if (multiplayerManager.GetCurrentAIPlayerList().Count > 0)
            {
                Debug.Log("multiplayerManager.GetCurrentAIPlayerList " + multiplayerManager.GetCurrentAIPlayerList().Count);

                ulong aiClientId = GenerateUniqueAIClientId();
                var fungalIndex = UnityEngine.Random.Range(0, fungalCollection.Fungals.Count);
                AddPlayer(aiClientId, 1, fungalIndex, isAI: true);
            }

            OnSpawnerReady?.Invoke(this);
        }
    }


    public void AddPlayer(ulong clientId, int playerIndex, int fungalIndex, bool isAI = false)
    {
        Debug.Log("AddPlayer");
        OnPlayerAddedServerRpc(clientId, playerIndex, fungalIndex, isAI);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerAddedServerRpc(ulong clientId, int playerIndex, int fungalIndex, bool isAI = false)
    {
        Debug.Log("OnPlayerAddedServerRpc");

       
        var spawnOrigin = arena.SpawnPositions[playerIndex].position;

        var spawnPosition = spawnOrigin;

        var networkFungal = Instantiate(fungalPrefab, spawnPosition, Quaternion.identity);
        networkFungal.Initialize(playerIndex, fungalIndex);
        networkFungal.NetworkObject.SpawnWithOwnership(clientId);

        if (!isAI)
        {
            OnPlayerAddedClientRpc(clientId, playerIndex, networkFungal.NetworkObjectId, isAI);
        }
    }

    [ClientRpc]
    private void OnPlayerAddedClientRpc(ulong clientId, int playerIndex, ulong networkObjectId,  bool isAi)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            var networkFungal = networkObject.GetComponent<NetworkFungal>();

            if (networkFungal.IsOwner)
            {
                var fishingPlayer = FindObjectOfType<FishingPlayer>();
                fishingPlayer.AssignFungal(networkFungal);
            }

            pufferballReference.AddPlayer(clientId, playerIndex, networkFungal, isAi);
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
                pufferballReference.AddPlayer(clientId, networkFungal.Index, networkFungal, isAi);
            }
        }
    }

    private ulong GenerateUniqueAIClientId()
    {
        // Example strategy: ulong.MaxValue - aiPlayers.Count
        return ulong.MaxValue - (ulong)aiPlayers.Count;
    }

    private List<ulong> aiPlayers = new();
}
