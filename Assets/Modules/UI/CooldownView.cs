using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CooldownView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text cooldownText; // Reference to cooldown text
    [SerializeField] private Image cooldownRadial; // Reference to radial fill Image
    [SerializeField] private Color startColor = Color.red; // Color at start of cooldown
    [SerializeField] private Color endColor = Color.white; // Color at end of cooldown

    private void Awake()
    {
        cooldownRadial.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false); // Hide text initially
        cooldownRadial.fillAmount = 0; // Ensure the radial fill is empty
    }

    public void StartCooldown(float cooldownTime)
    {
        StartCoroutine(CooldownRoutine(cooldownTime));
    }

    private IEnumerator CooldownRoutine(float cooldownTime)
    {
        button.interactable = false;
        cooldownRadial.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true); // Show cooldown text
        cooldownRadial.fillAmount = 1; // Start with full fill

        float timeLeft = cooldownTime;
        while (timeLeft > 0)
        {
            float progress = timeLeft / cooldownTime; // Calculate progress (1 to 0)
            cooldownText.text = Mathf.CeilToInt(timeLeft).ToString(); // Display integer countdown
            cooldownRadial.fillAmount = progress; // Update radial fill
            cooldownText.color = Color.Lerp(endColor, startColor, progress); // Lerp text color

            yield return null;
            timeLeft -= Time.deltaTime;
        }

        cooldownRadial.fillAmount = 0; // Hide radial fill
        cooldownText.gameObject.SetActive(false); // Hide text after cooldown
        cooldownRadial.gameObject.SetActive(false);
        button.interactable = true;
    }
}
