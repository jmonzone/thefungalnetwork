using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CooldownModel
{
    public float Cooldown { get; private set; }
    public bool IsOnCooldown { get; private set; }

    // Event to notify when cooldown progress is updated
    public event System.Action<float> OnCooldownUpdate;
    public event System.Action OnCooldownComplete;

    public CooldownModel(float cooldown)
    {
        Cooldown = cooldown;
    }

    public IEnumerator StartCooldown()
    {
        if (!IsOnCooldown)
        {
            IsOnCooldown = true;
            float timeLeft = Cooldown;

            while (timeLeft > 0)
            {
                float progress = timeLeft / Cooldown;

                // Invoke the event to notify listeners
                OnCooldownUpdate?.Invoke(progress);

                timeLeft -= Time.deltaTime;
                yield return null;
            }

            IsOnCooldown = false;
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
        this.ability = ability;
        ability.OnAvailabilityChanged += () => SetInteractable(ability.IsAvailable);
        ability.Cooldown.OnCooldownUpdate += OnCooldownUpdate;
        ability.Cooldown.OnCooldownComplete += OnCooldownComplete;
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
        cooldownText.text = Mathf.CeilToInt(progress * 100).ToString();
        cooldownRadial.fillAmount = progress;
        cooldownText.color = Color.Lerp(endColor, startColor, progress);
    }

    private void OnCooldownComplete()
    {
        SetInteractable(true);
        cooldownText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (ability != null)
        {
            ability.Cooldown.OnCooldownUpdate -= OnCooldownUpdate;
            ability.Cooldown.OnCooldownComplete -= OnCooldownComplete;
        }
    }
}
