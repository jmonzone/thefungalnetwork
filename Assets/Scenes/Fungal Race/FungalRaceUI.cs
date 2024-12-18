using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FungalRaceUI : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider pitchSlider;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Button burstButton;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float maxCharge = 0.05f;
    [SerializeField] private float minCharge = 0.005f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.1f;
    [SerializeField] private float burstEnergyCost;
    [SerializeField] private float burstEnergyRate;
    
    [SerializeField] private float charge;
    [SerializeField] private float chargeRate;
    [SerializeField] private bool isBursting = false;

    [SerializeField] private float auraValue = 0;

    public float AuraValue => auraValue;

    private float burstStartCharge;

    public event UnityAction OnAuraTypeChanged;

    private void Awake()
    {
        burstButton.onClick.AddListener(() =>
        {
            burstStartCharge = charge;
            isBursting = true;
        });

        pitchSlider.onValueChanged.AddListener(_ =>
        {
            OnAuraTypeChanged?.Invoke();
        });
    }

    private void Update()
    {
        canvasGroup.interactable = !isBursting;
        canvasGroup.alpha = isBursting ? 0.75f : 1f;
        burstButton.interactable = !isBursting && charge >= burstEnergyCost;

        float movementSpeed;

        if (isBursting)
        {
            movementSpeed = maxSpeed;
            charge -= burstEnergyRate * Time.deltaTime;
            audioSource.pitch = maxPitch;
            controller.Movement.SetSpeed(movementSpeed);

            if (burstStartCharge - charge > burstEnergyCost) isBursting = false;
        }
        else
        {
            // Normalize the slider value to the range [0, 1]
            auraValue = pitchSlider.value / pitchSlider.maxValue;

            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, auraValue);
            movementSpeed = Mathf.Lerp(minSpeed, maxSpeed, auraValue);

            chargeRate = Mathf.Lerp(minCharge, maxCharge, 1 - auraValue);

            charge += chargeRate;
            controller.Movement.SetSpeed(movementSpeed);
        }


        charge = Mathf.Clamp(charge, 0f, 1f);
        chargeSlider.value = charge;
    }
}
