using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;

    private Slider slider;

    public float Value => slider.value;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>(includeInactive: true);
        slider.maxValue = maxHealth;
        slider.minValue = 0;
        slider.value = maxHealth;
    }

    public void Damage()
    {
        slider.value--;
    }
}
