using Unity.Netcode;
using UnityEngine;

public class AbilityCastSync : NetworkBehaviour
{
    private AbilityCast abilityCast;
    private Attackable attackable;

    private void Awake()
    {
        abilityCast = GetComponent<AbilityCast>();
        attackable = GetComponent<Attackable>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            abilityCast.OnCastStart += OnAbilityCast;
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            abilityCast.OnCastStart -= OnAbilityCast;
        }
    }

    private void OnAbilityCast()
    {
        //todo: centralize logic with AbilityCastController
        RequestAbilityCastServerRpc(NetworkManager.Singleton.LocalClientId, abilityCast.StartPosition + Vector3.up * 0.5f, abilityCast.Direction);
    }

    [ServerRpc()]
    public void RequestAbilityCastServerRpc(ulong clientId, Vector3 spawnPosition, Vector3 direction)
    {
        // Only the server will execute this logic
        var networkProjectile = Instantiate(abilityCast.Data.NetworkPrefab, spawnPosition, Quaternion.LookRotation(direction), transform);
        networkProjectile.SpawnWithOwnership(clientId);

        SendAbilityInfoClientRpc(clientId, networkProjectile.NetworkObjectId, direction, abilityCast.Data.Speed);

    }

    [ClientRpc]
    private void SendAbilityInfoClientRpc(ulong clientId, ulong networkObjectId, Vector3 direction, float speed)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Retrieve the spawned object on the client using the NetworkObjectId
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                // Apply the rotated direction to the projectile
                networkObject.GetComponent<NetworkProjectile>().Shoot(direction, abilityCast.Data.MaxDistance, speed, attackable => this.attackable != attackable);
            }
            else
            {
                Debug.LogError("Failed to find the spawned object on the client.");
            }
        }
    }
}
