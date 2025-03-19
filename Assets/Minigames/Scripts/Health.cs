using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    private float currentShield;

    public float CurrentHealth => currentHealth.Value;
    public float CurrentShield => currentShield;
    public float MaxHealth => maxHealth;

    public event UnityAction OnDamaged;
    public event UnityAction OnHealthChanged;
    public event UnityAction OnHealthDepleted;
    public event UnityAction OnShieldChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner) Replenish();

        currentHealth.OnValueChanged += (previousValue, newValue) =>
        {
            OnHealthChanged?.Invoke();

            if (currentHealth.Value <= 0)
            {
                OnHealthDepleted?.Invoke();
            }
        };
    }

    public void Replenish()
    {
        OnReplenishServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnReplenishServerRpc()
    {
        currentHealth.Value = maxHealth;
    }

    public void Damage(float damage, ulong sourceId)
    {
        OnDamageServerRpc(damage, sourceId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDamageServerRpc(float damage, ulong sourceId)
    {
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, maxHealth);

        if (sourceId != NetworkObjectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(sourceId, out var networkObject))
            {
                var networkFungal = networkObject.GetComponent<NetworkFungal>();
                if (currentHealth.Value <= 0) networkFungal.Score.Value += 250f;
                else networkFungal.Score.Value += 35f;
            }
        }

        OnDamageClientRpc();
    }

    [ClientRpc]
    private void OnDamageClientRpc()
    {
        OnDamaged?.Invoke();
    }

    public void SetShield(float shield)
    {
        currentShield = Mathf.Max(0, shield);
        OnShieldChanged?.Invoke();
    }
}
