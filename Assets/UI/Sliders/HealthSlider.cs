using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    private Slider slider;
    private Attackable attackable;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>(includeInactive: true);
        attackable = GetComponentInParent<Attackable>();


        slider.maxValue = attackable.MaxHealth;
        slider.minValue = 0;
        slider.value = attackable.CurrentHealth;

        attackable.OnHealthChanged += UpdateView;
    }

    private void Start()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        slider.value = attackable.CurrentHealth;
        slider.gameObject.SetActive(attackable.CurrentHealth != attackable.MaxHealth && attackable.CurrentHealth != 0);
    }
}
