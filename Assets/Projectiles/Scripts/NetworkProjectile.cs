using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] private Projectile projectile;

    private NetworkVariable<bool> dissapatedNetwork = new NetworkVariable<bool>(true);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            projectile.OnDissipateStart += () => OnProjectileDissipateServerRpc();
            projectile.OnComplete += () => OnProjectileCompleteServerRpc();
        }
        else
        {
            if (!dissapatedNetwork.Value)
            {
                projectile.HideProjectileParticles();
            }
        }
    }

    public void Shoot(Vector3 direction, float maxDistance, float speed, Func<Attackable, bool> isValidTarget)
    {
        projectile.Shoot(direction, maxDistance, speed, isValidTarget);
    }

    [ServerRpc]
    private void OnProjectileDissipateServerRpc()
    {
        dissapatedNetwork.Value = false;
        OnProjectileDissipateClientRpc();
    }

    [ClientRpc]
    private void OnProjectileDissipateClientRpc()
    {
        if (!IsOwner) projectile.StartDisspate();
    }

    [ServerRpc]
    private void OnProjectileCompleteServerRpc()
    {
        dissapatedNetwork.Value = false;
        OnProjectileCompleteClientRpc();
    }

    [ClientRpc]
    private void OnProjectileCompleteClientRpc()
    {
        if (!IsOwner) projectile.EndAnimation();
    }
}
