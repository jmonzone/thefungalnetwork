using Unity.Netcode;
using UnityEngine;

public class NetworkPufferball : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        var movement = GetComponent<MovementController>();

        var direction = new Vector3(-1, 0, 1);
        movement.SetDirection(direction);
    }
}
