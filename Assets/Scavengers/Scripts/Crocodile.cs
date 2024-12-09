using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crocodile : MonoBehaviour
{
    [SerializeField] private GameObject overheadUI;
    private Slider slider;

    private void Awake()
    {
        overheadUI.SetActive(true);

        slider = GetComponentInChildren<Slider>(includeInactive: true);
        slider.maxValue = 10f;
        slider.minValue = 0;
        slider.value = 10f;
    }

    public void Damage()
    {
        slider.value--;
    }
}
