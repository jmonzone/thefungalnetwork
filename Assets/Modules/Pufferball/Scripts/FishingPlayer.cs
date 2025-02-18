using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class FishingPlayer : NetworkBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private PlayerReference player;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;

    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private FishingRodProjectile fishingRodPrefab;

    private NetworkFungal networkFungal;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            var characterIndex = Random.Range(0, fungalCollection.Fungals.Count);

            if (multiplayer.JoinedLobby != null)
            {
                // Get the ID of the local player
                string localPlayerId = AuthenticationService.Instance.PlayerId;

                // Find the local player in the lobby's player list
                var localPlayer = multiplayer.JoinedLobby.Players.FirstOrDefault(player => player.Id == localPlayerId);

                characterIndex = localPlayer.Data.TryGetValue("Fungal", out var fungalData)
                        ? int.TryParse(fungalData?.Value, out var index) ? index : 0 : 0;
            }

            RequestSpawnFungalServerRpc(NetworkManager.Singleton.LocalClientId, characterIndex);
            RequestSpawnFishingRodServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }



    private int GetPlayerIndex(ulong clientId)
    {
        var connectedClients = NetworkManager.Singleton.ConnectedClientsList
            .OrderBy(client => client.ClientId) // Ensure ordered list
            .Select(client => client.ClientId)
            .ToList();

        return connectedClients.IndexOf(clientId); // 0-based index
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnFungalServerRpc(ulong clientId, int characterIndex)
    {
        var fungal = fungalCollection.Fungals[characterIndex];

        var randomOffset = Random.insideUnitSphere.normalized;
        randomOffset.y = 0;

        var playerIndex = GetPlayerIndex(clientId);
        var spawnPosition = arena.SpawnPositions[playerIndex];

        var networkFungal = Instantiate(fungal.NetworkPrefab, spawnPosition.position, Quaternion.identity, transform);
        networkFungal.NetworkObject.SpawnWithOwnership(clientId);


        OnFungalSpawnedClientRpc(clientId, networkFungal.NetworkObjectId);
    }

    [ClientRpc]
    private void OnFungalSpawnedClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                networkFungal = networkObject.GetComponent<NetworkFungal>();
                player.SetTransform(networkFungal.transform);
                navigation.Navigate(inputView);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnFishingRodServerRpc(ulong clientId)
    {
        var networkFishingRod = Instantiate(fishingRodPrefab, Vector3.zero, Quaternion.identity, transform);
        networkFishingRod.NetworkObject.SpawnWithOwnership(clientId);

        OnFishingRodSpawnedClientRpc(clientId, networkFishingRod.NetworkObjectId);
    }

    [ClientRpc]
    private void OnFishingRodSpawnedClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                var fishingRod = networkObject.GetComponent<FishingRodProjectile>();
                pufferballReference.Initialize(fishingRod);
            }
        }
    }
}
