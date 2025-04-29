using Unity.Netcode;

public class NetworkPowerUp : NetworkBehaviour
{
    private PowerUp powerUp;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        powerUp = GetComponent<PowerUp>();
        powerUp.HandleCollection = fungal => HandleCollectionServerRpc(fungal.Id);
        powerUp.HandleRespawn = HandleRespawnServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleCollectionServerRpc(ulong fungalId)
    {
        HandleCollectionClientRpc(fungalId);
    }

    [ClientRpc]
    private void HandleCollectionClientRpc(ulong fungalId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fungalId, out var networkObject))
        {
            var fungal = networkObject.GetComponent<FungalController>();
            powerUp.ApplyCollectLogic(fungal);
            if (IsOwner) powerUp.StartRespawn();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void HandleRespawnServerRpc()
    {
        HandleRespawnClientRpc();
    }

    [ClientRpc]
    private void HandleRespawnClientRpc()
    {
        powerUp.ApplyRespawn();
    }
}
