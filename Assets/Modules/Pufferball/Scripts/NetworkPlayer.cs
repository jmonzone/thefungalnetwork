using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Controller controller;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;

    [SerializeField] private FungalCollection fungalCollection;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            var initialIndex = Random.Range(0, fungalCollection.Fungals.Count);
            RequestSpawnFungalServerRpc(NetworkManager.Singleton.LocalClientId, Random.Range(0, initialIndex));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnFungalServerRpc(ulong clientId, int fungalIndex)
    {
        var fungal = fungalCollection.Fungals[fungalIndex];

        var randomOffset = Random.insideUnitSphere.normalized * 3f;
        randomOffset.y = 0;

        var randomPosition = arena.PlayerSpawnPosition + randomOffset;

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
                var networkFungal = networkObject.GetComponent<NetworkFungal>();
                controller.SetMovement(networkFungal.Movement);
                navigation.Navigate(inputView);
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
        }
    }
}
