using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private GameObject render;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image shieldFill;

    [SerializeField] private Color healthColor = Color.green;
    [SerializeField] private Color shieldColor = Color.blue;

    private Health health;

    private void Awake()
    {
        health = GetComponentInParent<Health>();

        health.OnHealthChanged += _ => UpdateView();
        health.OnShieldChanged += UpdateView;

        healthFill.color = healthColor;
        shieldFill.color = shieldColor;
    }

    private void Start()
    {
        healthFill.fillAmount = 1;
        shieldFill.fillAmount = 0;
    }

    private void UpdateView()
    {
        render.SetActive(health.CurrentHealth > 0);

        float healthRatio = health.CurrentHealth / (health.MaxHealth + health.CurrentShield);
        float shieldRatio = (health.CurrentHealth  + health.CurrentShield) / (health.MaxHealth + health.CurrentShield);

        // Update fill sizes
        healthFill.fillAmount = healthRatio;
        shieldFill.fillAmount = shieldRatio;

        // Ensure colors are applied
        healthFill.color = healthColor;
        shieldFill.color = shieldColor;
    }
}
