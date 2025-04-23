using Unity.Netcode;
using UnityEngine;

public class NetworkHealth : NetworkBehaviour
{
    private Health health;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //Debug.Log($"HealthOnNetworkSpawn {IsOwner}");

        health = GetComponent<Health>();
        health.OnDamageRequested -= health.ApplyDamage;
        health.OnDamageRequested += OnDamageServerRpc;
        health.OnHealthChangeRequested -= health.ApplyHealthChange;
        health.OnHealthChangeRequested += OnHealthChangeRequestedServerRpc;
        health.OnShieldChangeRequested -= health.ApplyShieldChange;
        health.OnShieldChangeRequested += OnShieldRequestedServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDamageServerRpc(DamageEventArgs args)
    {
        var fungal = GetComponentInParent<NetworkFungal>();

        if (args.source != NetworkObjectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(args.source, out var networkObject))
            {
                var networkFungal = networkObject.GetComponent<NetworkFungal>();

                if (args.lethal) networkFungal.OnKillServerRpc(transform.position, fungal.Index);
                else networkFungal.AddToScoreServerRpc(new ScoreEventArgs
                {
                    value = 35f,
                    position = transform.position,
                    label = "Hit"
                });
            }
        }

        OnDamageClientRpc(args);
    }

    [ClientRpc]
    public void OnDamageClientRpc(DamageEventArgs args)
    {
        health.ApplyDamage(args);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnHealthChangeRequestedServerRpc(float value)
    {
        OnHealthChangeRequestedClientRpc(value);
    }

    [ClientRpc]
    private void OnHealthChangeRequestedClientRpc(float value)
    {
        health.ApplyHealthChange(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnShieldRequestedServerRpc(float shield)
    {
        OnShieldRequestedClientRpc(shield);
    }

    [ClientRpc]
    private void OnShieldRequestedClientRpc(float shield)
    {
        health.ApplyShieldChange(shield);
    }
}
