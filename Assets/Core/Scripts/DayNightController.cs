using System;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private Light targetLight;
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;

    [Header("Developer Options")]
    [SerializeField] private bool debug = false;
    [SerializeField] [Range(0, 24)] private int hour;

    private void Update()
    {
        var time = (debug ? hour : DateTime.Now.Hour) / 24f;

        if (time < 0.5)
        {
            targetLight.color = Color.Lerp(nightColor, dayColor, time / 0.5f);
        }
        else
        {
            targetLight.color = Color.Lerp(dayColor, nightColor, (time - 0.5f) / 0.5f);
        }
    }
}
