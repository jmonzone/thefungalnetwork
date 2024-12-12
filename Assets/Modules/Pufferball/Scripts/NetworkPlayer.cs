using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkAvatarPrefab;
    [SerializeField] private NetworkObject networkCrocdilePrefab;

    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Possession possesionService;
    [SerializeField] private Controller controller;
    [SerializeField] private ViewReference inputView;

    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;

    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private ShruneCollection shruneCollection;

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
                    controller.SetController(spawnedFungal.GetComponent<Controllable>());
                }
                else
                {
                    var randomOffset = Random.insideUnitSphere;
                    randomOffset.y = 0;

                    var avatarSpawnPosition = arena.PlayerSpawnPosition + randomOffset.normalized * 2f;
                    var spawnedAvatar = Instantiate(networkAvatarPrefab, avatarSpawnPosition, forwardRotation, transform);
                    spawnedAvatar.Spawn();
                    controller.SetController(spawnedAvatar.GetComponent<Controllable>());
                }


                var spawnedCrocodile = Instantiate(networkCrocdilePrefab, arena.CrocodileSpawnPosition, forwardRotation, transform);
                spawnedCrocodile.Spawn();
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
            inputView.RequestShow();
        }

        if (IsOwner)
        {
            abilityCast.OnComplete += OnAbilityCast;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            abilityCast.OnComplete -= OnAbilityCast;
        }
    }

    private void OnAbilityCast()
    {
        //todo: centralize logic with AbilityCastController
        RequestAbilityCastServerRpc(NetworkManager.Singleton.LocalClientId, abilityCast.ShruneId, abilityCast.StartPosition + Vector3.up * 0.5f, abilityCast.Direction);
    }

    [ServerRpc()]
    public void RequestAbilityCastServerRpc(ulong clientId, string shruneId, Vector3 spawnPosition, Vector3 direction)
    {
        var targetShrune = shruneCollection.Data.Find(shrune => shrune.name == shruneId);
        if (!targetShrune) return;

        // Only the server will execute this logic
        var networkProjectile = Instantiate(targetShrune.NetworkPrefab, spawnPosition, Quaternion.LookRotation(direction), transform);
        networkProjectile.SpawnWithOwnership(clientId);

        SendAbilityInfoClientRpc(clientId, networkProjectile.NetworkObjectId, direction);

    }

    [ClientRpc]
    private void SendAbilityInfoClientRpc(ulong clientId, ulong networkObjectId, Vector3 direction)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Retrieve the spawned object on the client using the NetworkObjectId
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                // Apply the rotated direction to the projectile
                networkObject.GetComponent<NetworkProjectile>().Shoot(direction, abilityCast.MaxDistance, attackable => attackable != controller.Attackable );
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
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
                controller.SetController(networkFungal.Controllable);
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
        }
    }
}
