using UnityEngine;
using UnityEngine.Events;

public class Attackable : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    public event UnityAction OnCurrentHealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void Damage()
    {
        if (CurrentHealth > 0)
        {
            CurrentHealth--;
            OnCurrentHealthChanged?.Invoke();
        }
    }
}
