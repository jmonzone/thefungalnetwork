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
        var fungalClientId = fishController.Fungal.GetComponent<NetworkObject>().OwnerClientId;

        Debug.Log($"FishController_OnPickedUp {fungalClientId}");

        FishController_OnPickedUpServerRpc(fungalClientId);
    }

    [ServerRpc]
    private void FishController_OnPickedUpServerRpc(ulong fungalClientId)
    {
        Debug.Log($"FishController_OnPickedUpServerRpc {fungalClientId}");
        NetworkObject.ChangeOwnership(fungalClientId);
        FishController_OnPickedUpClientRpc();
    }

    [ClientRpc]
    private void FishController_OnPickedUpClientRpc()
    {
        Debug.Log($"FishController_OnPickedUpClientRpc");

    }
}