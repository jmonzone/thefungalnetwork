using Unity.Netcode;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] private Projectile projectile;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            projectile.OnDissipate += () => OnProjectileDissipateClientRpc();
            projectile.OnComplete += () => OnProjectileCompleteServerRpc();
        }
    }

    public void Shoot(Vector3 direction, float maxDistance)
    {
        projectile.Shoot(direction, maxDistance);
    }

    [ServerRpc]
    private void OnProjectileCompleteServerRpc()
    {
        Debug.Log("network projectile server rpc");
        OnProjectileCompleteClientRpc();
    }

    [ClientRpc]
    private void OnProjectileCompleteClientRpc()
    {
        Debug.Log("network projectile client rpc");
        if (!IsOwner) projectile.EndAnimation();
    }

    [ClientRpc]
    private void OnProjectileDissipateClientRpc()
    {
        Debug.Log("network projectile dissapate client rpc");
        if (!IsOwner) projectile.StartDisspate();
    }
}
