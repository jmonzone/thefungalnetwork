using Unity.Netcode;
using UnityEngine;

public class NetworkAttackable : NetworkBehaviour
{
    private Attackable attackable;
    public NetworkVariable<float> CurrentHealth = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        attackable = GetComponent<Attackable>();

        if (IsServer)
        {
            // Initialize health only on the server
            CurrentHealth.Value = attackable.CurrentHealth; // Initial value
        }

        // Owner-specific logic
        if (IsOwner)
        {
            attackable.OnCurrentHealthChanged += () =>
            {
                CurrentHealth.Value = attackable.CurrentHealth;
            };
        }
        else
        {
            // Update health locally when CurrentHealth changes
            CurrentHealth.OnValueChanged += (oldValue, newValue) =>
            {
                Debug.Log($"Health updated from {oldValue} to {newValue}");
                attackable.SetHealth(newValue);
            };
        }
    }
}
