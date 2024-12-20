using Unity.Netcode;
using UnityEngine;

public class NetworkAttackable : NetworkBehaviour
{
    private Attackable attackable;
    public NetworkVariable<float> CurrentHealth = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        attackable = GetComponent<Attackable>();

        if (IsServer)
        {
            // Initialize health only on the server
            CurrentHealth.Value = attackable.CurrentHealth; // Initial value
        }
        else
        {
            // Update health locally when CurrentHealth changes
            CurrentHealth.OnValueChanged += (oldValue, newValue) =>
            {
                Debug.Log($"Health updated from {oldValue} to {newValue}");
                attackable.SetHealth(newValue);
            };

            attackable.SetHealth(CurrentHealth.Value);
        }

        attackable.OnDamaged += () =>
        {
            SyncHealthToOthersClientRpc(NetworkManager.Singleton.LocalClientId);
        };
    }

    [ClientRpc]
    public void SyncHealthToOthersClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        attackable.Damage();
    }
}
