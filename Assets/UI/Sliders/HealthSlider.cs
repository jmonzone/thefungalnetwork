using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    private Slider slider;
    private Health health;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>(includeInactive: true);
        health = GetComponentInParent<Health>();

        slider.maxValue = health.MaxHealth;
        slider.minValue = 0;
        slider.value = health.CurrentHealth;

        health.OnHealthChanged += UpdateView;
    }

    private void Start()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        slider.value = health.CurrentHealth;
        slider.gameObject.SetActive(health.CurrentHealth > 0 && health.CurrentHealth < health.MaxHealth);
    }
}
