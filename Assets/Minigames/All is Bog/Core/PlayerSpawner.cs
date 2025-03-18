using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo
{
    public ulong ClientId;
    public bool IsAI;
    public int FungalIndex;

    public PlayerInfo(ulong clientId, int fungalIndex, bool isAI = false)
    {
        ClientId = clientId;
        FungalIndex = fungalIndex;
        IsAI = isAI;
    }
}

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private bool addAIPlayer = false;

    private List<PlayerInfo> currentPlayers = new();

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


    [ClientRpc]
    private void OnFungalSpawnedClientRpc(ulong networkObjectId, int playerIndex)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            var networkFungal = networkObject.GetComponent<NetworkFungal>();
            pufferballReference.RegisterPlayer(networkFungal, playerIndex);

            if (networkFungal.IsOwner)
            {
                networkFungal.InitializeServerRpc(playerIndex);

                var fishingPlayer = FindObjectOfType<FishingPlayer>();
                fishingPlayer.AssignFungal(networkFungal);
            }
        }

        Debug.Log("Client owner spawned, searching for existing NetworkFungals...");

        // Find all existing NetworkFungal objects using the SpawnManager
        foreach (var spawnedObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            var networkFungal = spawnedObject.GetComponent<NetworkFungal>();
            if (networkFungal != null)
            {
                Debug.Log($"Registering fungal {networkFungal.name} with player index {networkFungal.PlayerIndex}");

                // Register the fungal with the pufferballReference
                pufferballReference.RegisterPlayer(networkFungal, networkFungal.PlayerIndex);
            }
        }
    }

    private ulong GenerateUniqueAIClientId()
    {
        // Example strategy: ulong.MaxValue - aiPlayers.Count
        return ulong.MaxValue - (ulong)aiPlayers.Count;
    }

    private List<ulong> aiPlayers = new();

    public void AddPlayer(ulong clientId, int playerIndex, int fungalIndex, bool isAI = false)
    {
        Debug.Log("AddPlayer");
        SpawnFungalForPlayerServerRpc(clientId, playerIndex, fungalIndex, isAI);
    }

    public void RemovePlayer(ulong clientId)
    {
        var player = currentPlayers.FirstOrDefault(p => p.ClientId == clientId);
        if (player != null)
        {
            currentPlayers.Remove(player);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnFungalForPlayerServerRpc(ulong clientId, int playerIndex, int fungalIndex, bool isAI = false)
    {
        Debug.Log("SpawnFungalForPlayer");

        var playerInfo = new PlayerInfo(clientId, fungalIndex, isAI);
        currentPlayers.Add(playerInfo);

        var fungal = fungalCollection.Fungals[playerInfo.FungalIndex];
        var spawnOrigin = arena.SpawnPositions[0].position;

        int numberOfPlayers = currentPlayers.Count; // Number of players to be spawned

        // Calculate the spawn radius, with a dynamic range based on the number of players
        float spawnRadius = Mathf.Lerp(0f, 5f, Mathf.Clamp01((numberOfPlayers - 1) / 7f)); // Smooth increase with more players

        // Calculate the angle between players, evenly distributing them around the circle
        float angleStep = 360f / numberOfPlayers;
        float angle = angleStep * playerIndex;

        // Calculate X and Z positions using trigonometry
        float x = spawnOrigin.x + Mathf.Cos(Mathf.Deg2Rad * angle) * spawnRadius;
        float z = spawnOrigin.z + Mathf.Sin(Mathf.Deg2Rad * angle) * spawnRadius;

        Vector3 spawnPosition = new Vector3(x, spawnOrigin.y, z);

        var networkFungal = Instantiate(fungal.NetworkPrefab, spawnPosition, Quaternion.identity);
        networkFungal.NetworkObject.SpawnWithOwnership(playerInfo.ClientId);

        if (!playerInfo.IsAI)
        {
            OnFungalSpawnedClientRpc(networkFungal.NetworkObjectId, playerIndex);
        }
    }
}
