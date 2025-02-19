using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public event UnityAction OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        OnHealthChanged?.Invoke();
    }
}
