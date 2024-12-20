using UnityEngine;
using UnityEngine.Events;

public class Attackable : MonoBehaviour
{
    [SerializeField] private float currentHealth = 3f;
    [SerializeField] private float maxHealth = 3f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public event UnityAction OnDamageRequest;
    public event UnityAction OnDamaged;
    public event UnityAction OnHealthChanged;
    public event UnityAction OnHealthDepleted;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Restore()
    {
        SetHealth(maxHealth);
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Max(health, 0);
        OnHealthChanged?.Invoke();

        if (CurrentHealth <= 0f)
        {
            OnHealthDepleted?.Invoke();
        }
    }

    public void RequestDamage(float damage = 1)
    {
        if (HandleDamage())
        {
            OnDamageRequest?.Invoke();
        }
    }

    public bool HandleDamage()
    {
        if (CurrentHealth > 0)
        {
            SetHealth(currentHealth - 1);
            OnDamaged?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
}
