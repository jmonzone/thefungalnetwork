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

    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.1f;
    
    [SerializeField] private float auraValue = 0;

    public float AuraValue => auraValue;

    public event UnityAction OnAuraChanged;

    private void Awake()
    {
        pitchSlider.onValueChanged.AddListener(_ =>
        {
            OnAuraChanged?.Invoke();
        });
    }

    private void Update()
    {
        float movementSpeed;

        // Normalize the slider value to the range [0, 1]
        auraValue = pitchSlider.value / pitchSlider.maxValue;

        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, auraValue);
        movementSpeed = Mathf.Lerp(minSpeed, maxSpeed, auraValue);

        controller.Movement.SetSpeed(movementSpeed);
    }
}
