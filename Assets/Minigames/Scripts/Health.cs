using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private float currentShield;

    public float CurrentHealth => currentHealth;
    public float CurrentShield => currentShield;
    public float MaxHealth => maxHealth;

    public event UnityAction<int> OnHealthChanged;
    public event UnityAction<int> OnHealthDepleted;

    public event UnityAction OnShieldChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(float health, int source = -1)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(source);

        if (currentHealth <= 0)
        {
            OnHealthDepleted?.Invoke(source);
        }
    }

    public void SetShield(float shield)
    {
        currentShield = Mathf.Max(0, shield);
        OnShieldChanged?.Invoke();
    }
}
