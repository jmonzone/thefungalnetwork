using Unity.Netcode;
using UnityEngine;

public class NetworkFish : NetworkBehaviour
{
    private FishController fishController;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fishController = GetComponent<FishController>();
        fishController.OnPickedUp += FishController_OnPickedUp;
    }

    private void FishController_OnPickedUp()
    {
        var fungal = fishController.Fungal.GetComponent<NetworkObject>();

        Debug.Log($"FishController_OnPickedUp");

        FishController_OnPickedUpServerRpc(fungal.NetworkObjectId, fungal.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FishController_OnPickedUpServerRpc(ulong objectId, ulong clientId)
    {
        Debug.Log($"FishController_OnPickedUpServerRpc {clientId}");
        NetworkObject.ChangeOwnership(clientId);
        FishController_OnPickedUpClientRpc(objectId);
    }

    [ClientRpc]
    private void FishController_OnPickedUpClientRpc(ulong objectId)
    {
        Debug.Log($"FishController_OnPickedUpClientRpc");
        if (!IsOwner)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
            {
                var fungalController = networkObject.GetComponent<FungalController>();
                fishController.HandlePickup(fungalController);
            }
        }
    }
}