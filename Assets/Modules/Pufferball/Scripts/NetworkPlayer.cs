using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkAvatarPrefab;
    [SerializeField] private NetworkCrocodile networkCrocdilePrefab;

    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Possession possesionService;
    [SerializeField] private Controller controller;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;

    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;

    public NetworkVariable<Vector3> PlayerPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Update()
    {
        if (IsOwner && controller.Movement)
        {
            PlayerPosition.Value = controller.Movement.transform.position;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);

            var partner = possesionService.Fungal;
            var targetFungal = partner ? fungalInventory.Fungals.Find(fungal => fungal.Data.Id == partner.Data.Id) : null;

            Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

            if (IsServer)
            {
                if (targetFungal)
                {
                    var spawnedFungal = Instantiate(targetFungal.Data.NetworkPrefab, arena.PlayerSpawnPosition, forwardRotation, transform);
                    spawnedFungal.NetworkObject.Spawn();
                    spawnedFungal.Initialize(targetFungal.Data.Id);
                    controller.SetMovement(spawnedFungal.GetComponent<MovementController>());
                }
                else
                {
                    var randomOffset = Random.insideUnitSphere;
                    randomOffset.y = 0;

                    var avatarSpawnPosition = arena.PlayerSpawnPosition + randomOffset.normalized * 2f;
                    var spawnedAvatar = Instantiate(networkAvatarPrefab, avatarSpawnPosition, forwardRotation, transform);
                    spawnedAvatar.Spawn();
                    controller.SetMovement(spawnedAvatar.GetComponent<MovementController>());
                }


                //var spawnedCrocodile = Instantiate(networkCrocdilePrefab, arena.CrocodileSpawnPosition, Quaternion.LookRotation(Vector3.back), transform);
                //spawnedCrocodile.NetworkObject.Spawn();
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
            navigation.Navigate(inputView);
        }
    }

    

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnAvatarServerRpc(ulong clientId)
    {
        // Only the server will execute this logic
        var networkAvatar = Instantiate(networkAvatarPrefab, arena.PlayerSpawnPosition, Quaternion.identity, transform);
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
                var controllable = networkObject.GetComponent<MovementController>();
                Debug.Log("Received Controllable component on the client!");

                controller.SetMovement(controllable);
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
        var data = fungalCollection.Fungals.Find(fungal => fungal.Id == fungalId);
        if (!data) return;
        // Only the server will execute this logic
        var networkFungal = Instantiate(data.NetworkPrefab, arena.PlayerSpawnPosition, Quaternion.identity, transform);

        networkFungal.NetworkObject.SpawnWithOwnership(clientId);

        // Send the NetworkObject ID to the client
        SendFungalInfoClientRpc(clientId, networkFungal.NetworkObjectId, fungalId);
    }

    //todo: consolidate with sendavatar info
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
                controller.SetMovement(networkFungal.Movement);
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
        }
    }
}
