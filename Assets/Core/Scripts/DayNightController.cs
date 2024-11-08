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
        if (!debug) hour = DateTime.Now.Hour;

        if (time < 0.5)
        {
            var t = time / 0.5f;
            targetLight.color = Color.Lerp(nightColor, dayColor, t);
            targetLight.intensity = Mathf.Lerp(0.5f, 1, t);
        }
        else
        {
            var t = (time - 0.5f) / 0.5f;
            targetLight.color = Color.Lerp(dayColor, nightColor, t);
            targetLight.intensity = Mathf.Lerp(1, 0.5f, t);

        }
    }
}
