using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    private void Awake()
    {
        var slider = GetComponentInChildren<Slider>(includeInactive: true);
        var attackable = GetComponentInParent<Attackable>();

        attackable.OnCurrentHealthChanged += () =>
        {
            slider.value = attackable.CurrentHealth;
            slider.gameObject.SetActive(true);
        };

        attackable.OnDeath += () =>
        {
            slider.gameObject.SetActive(false);
        };

        slider.maxValue = attackable.MaxHealth;
        slider.minValue = 0;
        slider.value = attackable.CurrentHealth;
    }
}
