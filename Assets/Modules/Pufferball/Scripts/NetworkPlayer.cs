using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Controller controller;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;

    [SerializeField] private FungalCollection fungalCollection;

    private NetworkFungal networkFungal;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            var initialIndex = Random.Range(0, fungalCollection.Fungals.Count);

            if (multiplayer.JoinedLobby != null)
            {
                // Get the ID of the local player
                string localPlayerId = AuthenticationService.Instance.PlayerId;

                // Find the local player in the lobby's player list
                var localPlayer = multiplayer.JoinedLobby.Players.FirstOrDefault(player => player.Id == localPlayerId);

                initialIndex = localPlayer.Data.TryGetValue("Fungal", out var fungalData)
                        ? int.TryParse(fungalData?.Value, out var index) ? index : 0 : 0;
            }

            RequestSpawnFungalServerRpc(NetworkManager.Singleton.LocalClientId, initialIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnFungalServerRpc(ulong clientId, int fungalIndex)
    {
        var fungal = fungalCollection.Fungals[fungalIndex];

        var randomOffset = Random.insideUnitSphere.normalized;
        randomOffset.y = 0;

        var randomPosition = arena.PlayerSpawnPosition + randomOffset.normalized * 3f;

        var networkFungal = Instantiate(fungal.NetworkPrefab, randomPosition, Quaternion.identity, transform);
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
                controller.SetMovement(networkFungal.Movement);
                navigation.Navigate(inputView);
            }
        }
    }
}
