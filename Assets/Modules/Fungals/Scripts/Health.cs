using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public struct DamageEventArgs : INetworkSerializable
{
    public bool lethal;
    public int source;
    public int target;
    public bool SelfInflicted => source == target;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref lethal);
        serializer.SerializeValue(ref source);
        serializer.SerializeValue(ref target);
    }
}

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float currentShield = 0f;

    public float CurrentHealth => currentHealth;
    public float CurrentShield => currentShield;
    public float MaxHealth => maxHealth;

    public event UnityAction OnHealthChanged;
    public event UnityAction OnHealthDepleted;
    public event UnityAction<float> OnHealthChangeRequested;

    public event UnityAction<DamageEventArgs> OnDamaged;
    public event UnityAction<float, ulong> OnDamageRequested;

    public event UnityAction<float> OnShieldChangeRequested;
    public event UnityAction OnShieldChanged;

    private void Awake()
    {
        OnHealthChangeRequested += ApplyHealthChange;
        OnShieldChangeRequested += ApplyShieldChange;
    }

    public void SetHealth(float health)
    {
        OnHealthChangeRequested?.Invoke(health);
    }

    public void ReplenishHealth()
    {
        SetHealth(maxHealth);
    }

    public void ApplyHealthChange(float value)
    {
        currentHealth = value;

        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
        {
            OnHealthDepleted?.Invoke();
        }
    }

    public void Damage(float damage, ulong sourceId)
    {
        if (currentHealth <= 0) return;

        float remainingDamage = damage;

        // Step 1: Damage the shield first
        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(currentShield, remainingDamage);
            SetShield(currentShield - shieldDamage);
            remainingDamage -= shieldDamage;
        }

        // Step 2: If damage remains, apply it to health
        if (remainingDamage > 0)
        {
            SetHealth(Mathf.Clamp(currentHealth - remainingDamage, 0, maxHealth));
        }

        //Debug.Log($"Damage {damage} {sourceId}");
        OnDamageRequested?.Invoke(damage, sourceId);
    }

    public void ApplyDamage(DamageEventArgs args)
    {
        OnDamaged?.Invoke(args);
    }

    public void SetShield(float shield)
    {
        OnShieldChangeRequested?.Invoke(shield);
    }

    public void ApplyShieldChange(float shield)
    {
        currentShield = shield;
        OnShieldChanged?.Invoke();
    }
}
