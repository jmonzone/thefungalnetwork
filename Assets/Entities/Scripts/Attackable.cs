using UnityEngine;
using UnityEngine.Events;

public class Attackable : MonoBehaviour
{
    [SerializeField] private float currentHealth = 3f;
    [SerializeField] private float maxHealth = 3f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public event UnityAction OnDamaged;
    public event UnityAction OnCurrentHealthChanged;
    public event UnityAction OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        OnCurrentHealthChanged?.Invoke();

        if (CurrentHealth == 0f)
        {
            Debug.Log($"OnDeath {currentHealth}");

            OnDeath?.Invoke();
        }
    }

    public void Damage()
    {
        if (CurrentHealth > 0)
        {
            SetHealth(currentHealth - 1);
            OnDamaged?.Invoke();
        }
    }


}
