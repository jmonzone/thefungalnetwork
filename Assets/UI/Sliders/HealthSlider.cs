using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    private void Awake()
    {
        var slider = GetComponentInChildren<Slider>(includeInactive: true);
        var attackable = GetComponentInParent<Attackable>();

        attackable.OnHealthChanged += () =>
        {
            slider.value = attackable.CurrentHealth;
            slider.gameObject.SetActive(attackable.CurrentHealth != attackable.MaxHealth && attackable.CurrentHealth != 0);
        };

        slider.maxValue = attackable.MaxHealth;
        slider.minValue = 0;
        slider.value = attackable.CurrentHealth;
    }
}
