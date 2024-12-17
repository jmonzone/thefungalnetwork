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
    [SerializeField] private Button burstButton;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float baseMovementSpeed = 3f; // Maximum movement speed at 100% pitch
    [SerializeField] private float sprintMultiplier = 1.5f; // Maximum movement speed at 100% pitch
    [SerializeField] private float burstSpeedMultiplier = 2f;
    [SerializeField] private float burstEnergyDepletion = 1.5f;
    [SerializeField] private float burstEnergyPrice = 0.5f;
    [SerializeField] private float walkMultiplier = 0.5f; // Maximum movement speed at 100% pitch
    [SerializeField] private float lowPitchChargeSpeed = 4f;    // Intensity of charge curve parabola
    [SerializeField] private float baseChargeSpeed = 2f;
    [SerializeField] private float highPitchEnergyRate = 4f;    // Intensity of charge curve parabola
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.1f;
    
    [SerializeField] private float charge;
    [SerializeField] private float chargeRate;
    [SerializeField] private bool isBursting = false;

    private float burstStartCharge;
    private void Awake()
    {
        burstButton.onClick.AddListener(() =>
        {
            burstStartCharge = charge;
            isBursting = true;
        });
    }

    private void Update()
    {
        canvasGroup.interactable = !isBursting;
        canvasGroup.alpha = isBursting ? 0.75f : 1f;
        burstButton.interactable = !isBursting && charge >= burstEnergyPrice;

        float movementSpeed;

        if (isBursting)
        {
            movementSpeed = baseMovementSpeed * burstSpeedMultiplier;
            charge -= burstEnergyDepletion * Time.deltaTime;
            audioSource.pitch = maxPitch;
            controller.Movement.SetSpeed(movementSpeed);


            if (burstStartCharge - charge > burstEnergyPrice) isBursting = false;
        }
        else
        {
            // Normalize the slider value to the range [0, 1]
            float normalizedValue = pitchSlider.value / pitchSlider.maxValue;
            if (normalizedValue < 0.33f)
            {
                movementSpeed = baseMovementSpeed * walkMultiplier;
                charge += lowPitchChargeSpeed * Time.deltaTime;
                audioSource.pitch = minPitch;
            }
            else if (normalizedValue < 0.66f)
            {
                charge += baseChargeSpeed * Time.deltaTime;
                movementSpeed = baseMovementSpeed * ((charge * 0.25f) + 0.75f);
                audioSource.pitch = 1f;
            }
            else
            {
                charge -= highPitchEnergyRate * Time.deltaTime;
                audioSource.pitch = maxPitch;

                if (charge > 0) movementSpeed = baseMovementSpeed * sprintMultiplier;
                else movementSpeed = baseMovementSpeed * walkMultiplier;
            }
        }

        controller.Movement.SetSpeed(movementSpeed);

        charge = Mathf.Clamp(charge, 0f, 1f);
        chargeSlider.value = charge;
    }
}
