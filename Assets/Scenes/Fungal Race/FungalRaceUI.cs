using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FungalRaceUI : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider pitchSlider;
    [SerializeField] private Slider chargeSlider;


    [SerializeField] private float baseMovementSpeed = 3f; // Maximum movement speed at 100% pitch
    [SerializeField] private float sprintMultiplier = 1.5f; // Maximum movement speed at 100% pitch
    [SerializeField] private float walkMultiplier = 0.5f; // Maximum movement speed at 100% pitch
    [SerializeField] private float lowPitchChargeSpeed = 4f;    // Intensity of charge curve parabola
    [SerializeField] private float baseChargeSpeed = 2f;
    [SerializeField] private float highPitchEnergyRate = 4f;    // Intensity of charge curve parabola
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.1f;

    [SerializeField] private float charge;
    [SerializeField] private float chargeRate;

    private void Awake()
    {

    }

    private void Update()
    {
        chargeSlider.value = charge;

        // Normalize the slider value to the range [0, 1]
        float movementSpeed;
        float normalizedValue = pitchSlider.value / pitchSlider.maxValue;
        if (normalizedValue < 0.33f)
        {
            movementSpeed = baseMovementSpeed * walkMultiplier;
            charge += lowPitchChargeSpeed * Time.deltaTime;
            audioSource.pitch = minPitch;
        }
        else if (normalizedValue < 0.66f)
        {
            movementSpeed = baseMovementSpeed;
            charge += baseChargeSpeed * Time.deltaTime;
            audioSource.pitch = 1f;
        }
        else
        {
            movementSpeed = baseMovementSpeed * sprintMultiplier;
            charge -= highPitchEnergyRate * Time.deltaTime;
            audioSource.pitch = maxPitch;
        }

        charge = Mathf.Clamp(charge, 0f, 1f);


        controller.Movement.SetSpeed(movementSpeed * charge);



    }
}
