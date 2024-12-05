using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Possession possesionService;
    [SerializeField] private NetworkObject networkAvatarPrefab;
    [SerializeField] private Controller controller;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            Debug.Log("init player");

            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);

            var partner = possesionService.Fungal;
            var targetFungal = partner ? fungalInventory.Fungals.Find(fungal => fungal.Data.Id == partner.Data.Id) : null;

            Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

            if (IsServer)
            {
                if (targetFungal)
                {
                    var spawnedFungal = Instantiate(targetFungal.Data.NetworkPrefab, arena.SpawnPosition1, forwardRotation, transform);
                    spawnedFungal.NetworkObject.Spawn();
                    spawnedFungal.Initialize(targetFungal.Data.Id);
                    controller.SetController(spawnedFungal.GetComponent<Controllable>());
                }
                else
                {
                    var spawnedAvatar = Instantiate(networkAvatarPrefab, arena.SpawnPosition1, forwardRotation, transform);
                    spawnedAvatar.Spawn();
                    controller.SetController(spawnedAvatar.GetComponent<Controllable>());
                }
            }
            else
            {
                if (targetFungal)
                {
                    RequestSpawnFungalServerRpc(NetworkManager.Singleton.LocalClientId, targetFungal.Data.Id);
                }
                else
                {
                    RequestSpawnAvatarServerRpc(NetworkManager.Singleton.LocalClientId);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnAvatarServerRpc(ulong clientId)
    {
        // Only the server will execute this logic
        var networkAvatar = Instantiate(networkAvatarPrefab, arena.SpawnPosition1, Quaternion.identity, transform);
        networkAvatar.SpawnWithOwnership(clientId);

        // Send the NetworkObject ID to the client
        SendAvatarInfoClientRpc(clientId, networkAvatar.NetworkObjectId);
    }

    [ClientRpc]
    private void SendAvatarInfoClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Retrieve the spawned object on the client using the NetworkObjectId
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                // Get the desired component
                var controllable = networkObject.GetComponent<Controllable>();
                Debug.Log("Received Controllable component on the client!");

                controller.SetController(controllable);
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnFungalServerRpc(ulong clientId, string fungalId)
    {
        var data = fungalCollection.Data.Find(fungal => fungal.Id == fungalId);
        if (!data) return;
        // Only the server will execute this logic
        var networkFungal = Instantiate(data.NetworkPrefab, arena.SpawnPosition1, Quaternion.identity, transform);

        networkFungal.NetworkObject.SpawnWithOwnership(clientId);

        // Send the NetworkObject ID to the client
        SendFungalInfoClientRpc(clientId, networkFungal.NetworkObjectId, fungalId);
    }

    [ClientRpc]
    private void SendFungalInfoClientRpc(ulong clientId, ulong networkObjectId, string fungalName)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Retrieve the spawned object on the client using the NetworkObjectId
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                var networkFungal = networkObject.GetComponent<NetworkFungal>();
                Debug.Log("Received Controllable component on the client!");
                networkFungal.Initialize(fungalName);
                controller.SetController(networkFungal.Controllable);
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
        }
    }
}
