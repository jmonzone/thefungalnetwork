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
        health.OnDamageRequested += OnDamageServerRpc;
        health.OnHealthChangeRequested -= health.ApplyHealthChange;
        health.OnHealthChangeRequested += OnHealthChangeRequestedServerRpc;
        health.OnShieldChangeRequested -= health.ApplyShieldChange;
        health.OnShieldChangeRequested += OnShieldRequestedServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDamageServerRpc(float damage, ulong sourceId)
    {
        if (health.CurrentHealth <= 0) return;

        var knockout = health.CurrentHealth <= 0;

        var fungal = GetComponentInParent<NetworkFungal>();

        var args = new DamageEventArgs()
        {
            lethal = knockout,
            target = fungal.Index,
            source = fungal.Index,
        };

        if (sourceId != NetworkObjectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(sourceId, out var networkObject))
            {
                var networkFungal = networkObject.GetComponent<NetworkFungal>();

                args.source = networkFungal.Index;

                if (knockout) networkFungal.OnKillServerRpc(transform.position, fungal.Index);
                else networkFungal.AddToScoreServerRpc(new ScoreEventArgs
                {
                    value = 35f,
                    position = transform.position,
                    label = "Hit"
                });
            }
        }
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
