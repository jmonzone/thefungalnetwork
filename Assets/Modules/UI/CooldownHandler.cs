using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

[Serializable]
public class CooldownModel
{
    [SerializeField] private float cooldown;
    [SerializeField] private float remainingTime; 
    [SerializeField] private bool isOnCooldown;
    public float Cooldown => cooldown;
    public bool IsOnCooldown => isOnCooldown;
    public float RemainingTime => remainingTime;

    // Event to notify when cooldown progress is updated
    public event Action OnCooldownStart;
    public event Action<float> OnCooldownUpdate;
    public event Action OnCooldownComplete;

    public CooldownModel(float cooldown)
    {
        this.cooldown = cooldown;
    }

    public IEnumerator StartCooldown()
    {
        if (!IsOnCooldown)
        {
            isOnCooldown = true;
            remainingTime = Cooldown;
            OnCooldownStart?.Invoke();

            while (RemainingTime > 0)
            {
                float progress = RemainingTime / Cooldown;

                // Invoke the event to notify listeners
                OnCooldownUpdate?.Invoke(progress);

                remainingTime -= Time.deltaTime;
                yield return null;
            }

            OnCooldownUpdate?.Invoke(1);

            isOnCooldown = false;
            // Notify that cooldown is complete
            OnCooldownComplete?.Invoke();
        }
    }
}

public class CooldownHandler : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image cooldownRadial;
    [SerializeField] private Color startColor = Color.red;
    [SerializeField] private Color endColor = Color.white;

    private Ability ability;

    private void Awake()
    {
        cooldownRadial.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
        cooldownRadial.fillAmount = 0;
    }

    // Assign the CooldownModel externally
    public void AssignCooldownModel(Ability ability)
    {
        void OnAvailabilityChanged() => SetInteractable(ability.IsAvailable);

        if (this.ability)
        {
            this.ability.OnAvailabilityChanged -= OnAvailabilityChanged;
            this.ability.Cooldown.OnCooldownStart -= OnCooldownStart;
            this.ability.Cooldown.OnCooldownUpdate -= OnCooldownUpdate;
            this.ability.Cooldown.OnCooldownComplete -= OnCooldownComplete;
        }

        this.ability = ability;

        if (this.ability)
        {
            this.ability.OnAvailabilityChanged += OnAvailabilityChanged;
            this.ability.Cooldown.OnCooldownStart += OnCooldownStart;
            this.ability.Cooldown.OnCooldownUpdate += OnCooldownUpdate;
            this.ability.Cooldown.OnCooldownComplete += OnCooldownComplete;
            SetInteractable(true);
        }
        else
        {
            cooldownRadial.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);
        }
    }

    public void SetInteractable(bool value)
    {
        directionalButton.enabled = value;
        button.interactable = value;
        cooldownRadial.gameObject.SetActive(!value);
        cooldownRadial.fillAmount = value ? 0 : 1;
    }

    private void OnCooldownUpdate(float progress)
    {

        // Update the UI with the remaining time (in seconds)
        cooldownText.text = Mathf.CeilToInt(ability.Cooldown.RemainingTime).ToString();

        cooldownRadial.fillAmount = progress;
        cooldownText.color = Color.Lerp(endColor, startColor, progress);
    }

    private void OnCooldownStart()
    {
        SetInteractable(false);
        cooldownText.gameObject.SetActive(true);
    }

    private void OnCooldownComplete()
    {
        SetInteractable(ability.IsAvailable);
        cooldownText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (ability != null)
        {
            ability.Cooldown.OnCooldownStart -= OnCooldownStart;
            ability.Cooldown.OnCooldownUpdate -= OnCooldownUpdate;
            ability.Cooldown.OnCooldownComplete -= OnCooldownComplete;
        }
    }
}
