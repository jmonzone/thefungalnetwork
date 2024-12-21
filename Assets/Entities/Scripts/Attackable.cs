using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Attackable : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 3f;

    private NetworkVariable<float> networkHealth = new NetworkVariable<float>();
    public float CurrentHealth => networkHealth.Value;
    public float MaxHealth => maxHealth;

    public event UnityAction OnDamaged;
    public event UnityAction OnHealthChanged;
    public event UnityAction OnHealthDepleted;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            networkHealth.Value = maxHealth;
        }

        networkHealth.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"onvaluechanged {oldValue} {newValue}");
            OnHealthChanged?.Invoke();

            if (newValue <= 0f)
            {
                OnHealthDepleted?.Invoke();
            }
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestoreServerRpc()
    {
        networkHealth.Value = MaxHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(float damage)
    {
        networkHealth.Value = Mathf.Clamp(networkHealth.Value - damage, 0, maxHealth);
        DamageClientRpc();
    }

    [ClientRpc]
    private void DamageClientRpc()
    {
        OnDamaged?.Invoke();
    }
}
