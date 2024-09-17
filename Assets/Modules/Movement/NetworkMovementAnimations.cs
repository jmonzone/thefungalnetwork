using Unity.Netcode;
using UnityEngine;

public class NetworkMovementAnimations : NetworkBehaviour
{
    [SerializeField] private MovementAnimations animations;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animations.enabled = IsOwner;
    }
}
