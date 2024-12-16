using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FungalRaceUI : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private void Awake()
    {
        var pitchSlider = GetComponentInChildren<Slider>(includeInactive: true);
        pitchSlider.onValueChanged.AddListener(value =>
        {
            // Map the value using a cubic curve
            float curvedValue = Mathf.Pow(value / pitchSlider.maxValue, 1.5f); // Normalize by 3 and then cube
            float speed = Mathf.Lerp(0, 5f, curvedValue);

            controller.Movement.SetSpeed(speed);
        });
    }
}
