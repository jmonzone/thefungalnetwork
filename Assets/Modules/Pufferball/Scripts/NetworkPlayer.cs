using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Possession possesionService;
    [SerializeField] private NetworkObject networkAvatarPrefab;
    [SerializeField] private NetworkFungal networkFungalPrefab;
    [SerializeField] private Controller controller;

    private NetworkTransform networkTransform;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            Debug.Log("init player");
            networkTransform = GetComponent<NetworkTransform>();

            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            if (!TrySpawnPartner())
            {
                if (IsClient && !IsServer)
                {
                    RequestSpawnAvatarServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    var avatar = SpawnAvater();
                    controller.SetController(avatar.GetComponent<Controllable>());
                }
                
            }

            Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

            var randomPosition = Random.insideUnitSphere.normalized;
            randomPosition.y = 0;

            var spawnPosition = arena.SpawnPosition1 + randomPosition;
            networkTransform.Teleport(spawnPosition, forwardRotation, Vector3.one);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnAvatarServerRpc(ulong clientId)
    {
        // Only the server will execute this logic
        var avatar = Instantiate(networkAvatarPrefab, arena.SpawnPosition1, Quaternion.identity, transform);

        // Optionally, you can send information back to the requesting client if needed.
        var networkAvatar = avatar.GetComponent<NetworkObject>();
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


    private GameObject SpawnAvater()
    {
        var spawnedAvatar = Instantiate(networkAvatarPrefab, arena.SpawnPosition1, Quaternion.identity, transform);
        spawnedAvatar.Spawn();
        return spawnedAvatar.gameObject;
    }

    private bool TrySpawnPartner()
    {
        var partner = possesionService.Fungal;

        if (partner)
        {
            var targetFungal = fungalInventory.Fungals.Find(fungal => fungal.Data.Id == partner.Data.Id);

            if (targetFungal)
            {
                Debug.Log("target found");
                var spawnedFungal = Instantiate(networkFungalPrefab, arena.SpawnPosition1, Quaternion.identity, transform);
                spawnedFungal.NetworkObject.Spawn();
                spawnedFungal.Initialize(targetFungal.Data);
                return true;
            }

            Debug.Log("no target found");

        }

        return false;
    }
}
