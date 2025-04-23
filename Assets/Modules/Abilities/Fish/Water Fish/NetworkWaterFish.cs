using Unity.Netcode;
using UnityEngine;

public class NetworkWaterFish : NetworkBehaviour
{
    private WaterFish waterFish;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        waterFish = GetComponent<WaterFish>();

        waterFish.OnSpawnBubble -= waterFish.HandleSpawnBubble;
        waterFish.OnSpawnBubble += WaterFish_SpawnBubble;
    }

    private void WaterFish_SpawnBubble(Vector3 targetPosition)
    {
        SpawnBubbleServerRpc(targetPosition);
    }

    [ServerRpc]
    private void SpawnBubbleServerRpc(Vector3 targetPosition)
    {
        var bubble = waterFish.CreateBubble(targetPosition);
        bubble.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);

        HandleSpawnBubbleClientRpc();
    }

    [ClientRpc]
    private void HandleSpawnBubbleClientRpc()
    {
        if (IsOwner) waterFish.HandleBubbleSpawn();
    }
}
