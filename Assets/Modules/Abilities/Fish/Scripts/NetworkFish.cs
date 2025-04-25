using Unity.Netcode;
using UnityEngine;

public class NetworkFish : NetworkBehaviour
{
    private FishController fish;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fish = GetComponent<FishController>();
        fish.OnPickedUp += FishController_OnPickedUp;

        fish.OnRespawnComplete -= fish.HandleRespawn;
        fish.OnRespawnComplete += HandleRespawnServerRpc;

        fish.SetSpawnPosition(transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleRespawnServerRpc()
    {
        HandleRespawnClientRpc();
    }

    [ClientRpc]
    private void HandleRespawnClientRpc()
    {
        fish.HandleRespawn();
    }

    private void FishController_OnPickedUp()
    {
        var fungal = fish.Fungal.GetComponent<NetworkObject>();
        //Debug.Log($"FishController_OnPickedUp");

        FishController_OnPickedUpServerRpc(fungal.OwnerClientId, fungal.NetworkObjectId, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FishController_OnPickedUpServerRpc(ulong fungalClientId, ulong fungalObjectId, ulong pickupClientId)
    {
        //Debug.Log($"FishController_OnPickedUpServerRpc {fungalClientId}");
        NetworkObject.ChangeOwnership(fungalClientId);
        FishController_OnPickedUpClientRpc(fungalObjectId, pickupClientId);
    }

    [ClientRpc]
    private void FishController_OnPickedUpClientRpc(ulong objectId, ulong pickUpClientId)
    {
        //Debug.Log($"FishController_OnPickedUpClientRpc {NetworkManager.Singleton.LocalClientId} {pickUpClientId}");

        if (NetworkManager.Singleton.LocalClientId != pickUpClientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
            {
                Debug.Log($"FishController_OnPickedUpClientRpc");

                var fungalController = networkObject.GetComponent<FungalController>();
                fish.HandlePickup(fungalController);
            }
        }
    }
}