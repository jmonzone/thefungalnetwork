using Unity.Netcode;
using UnityEngine;

public class NetworkFish : NetworkBehaviour
{
    private FishController fish;
    private TelegraphTrajectory telegraphTrajectory;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fish = GetComponent<FishController>();
        fish.OnPickedUp += FishController_OnPickedUp;

        fish.ThrowFish.OnThrowStart += OnThrowStartServerRpc;
        fish.ThrowFish.OnThrowComplete += OnThrowCompleteServerRpc;

        telegraphTrajectory = GetComponent<TelegraphTrajectory>();
    }

    [ServerRpc]
    private void OnThrowStartServerRpc(Vector3 targetPosition)
    {
        OnThrowStartClientRpc(targetPosition);
    }

    [ClientRpc]
    private void OnThrowStartClientRpc(Vector3 targetPosition)
    {
        if (!IsOwner)
        {
            telegraphTrajectory.ShowIndicator(targetPosition, fish.ThrowFish.Radius);
        }
    }

    [ServerRpc]
    private void OnThrowCompleteServerRpc()
    {
        OnThrowCompleteClientRpc();
    }

    [ClientRpc]
    private void OnThrowCompleteClientRpc()
    {
        if (!IsOwner)
        {
            telegraphTrajectory.HideIndicator();
        }
    }

    private void FishController_OnPickedUp()
    {
        var fungal = fish.Fungal.GetComponent<NetworkObject>();
        Debug.Log($"FishController_OnPickedUp");

        FishController_OnPickedUpServerRpc(fungal.OwnerClientId, fungal.NetworkObjectId, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FishController_OnPickedUpServerRpc(ulong fungalClientId, ulong fungalObjectId, ulong pickupClientId)
    {
        Debug.Log($"FishController_OnPickedUpServerRpc {fungalClientId}");
        NetworkObject.ChangeOwnership(fungalClientId);
        FishController_OnPickedUpClientRpc(fungalObjectId, pickupClientId);
    }

    [ClientRpc]
    private void FishController_OnPickedUpClientRpc(ulong objectId, ulong pickUpClientId)
    {
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