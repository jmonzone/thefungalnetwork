using UnityEngine;
using UnityEngine.Events;

public class Attackable : MonoBehaviour
{
    [SerializeField] private float currentHealth = 3f;
    [SerializeField] private float maxHealth = 3f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

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

    public void Damage(float damage = 1)
    {
        if (CurrentHealth > 0)
        {
            SetHealth(currentHealth - damage);
            OnDamaged?.Invoke();
        }
    }


}
