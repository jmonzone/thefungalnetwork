using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    private void Awake()
    {
        var slider = GetComponentInChildren<Slider>();
        var attackable = GetComponentInParent<Attackable>();

        attackable.OnCurrentHealthChanged += () =>
        {
            slider.value = attackable.CurrentHealth;
            gameObject.SetActive(attackable.CurrentHealth > 0);
        };

        slider.maxValue = attackable.MaxHealth;
        slider.minValue = 0;
        slider.value = attackable.CurrentHealth;

        gameObject.SetActive(false);
    }
}
