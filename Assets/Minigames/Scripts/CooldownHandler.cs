using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CooldownHandler : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image cooldownRadial;
    [SerializeField] private Color startColor = Color.red;
    [SerializeField] private Color endColor = Color.white;

    private bool isOnCooldown;

    public bool IsOnCooldown => isOnCooldown;

    private void Awake()
    {
        cooldownRadial.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
        cooldownRadial.fillAmount = 0;
    }

    public void StartCooldown(float cooldownTime)
    {
        if (!isOnCooldown)
        {
            StartCoroutine(CooldownRoutine(cooldownTime));
        }
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
        cooldownRadial.gameObject.SetActive(!value);
        cooldownRadial.fillAmount = value ? 0 : 1;
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        isOnCooldown = true;
        SetInteractable(false);
        if (duration >= 1f) cooldownText.gameObject.SetActive(true);

        float timeLeft = duration;
        while (timeLeft > 0)
        {
            float progress = timeLeft / duration;
            cooldownText.text = Mathf.CeilToInt(timeLeft).ToString();
            cooldownRadial.fillAmount = progress;
            cooldownText.color = Color.Lerp(endColor, startColor, progress);

            yield return null;
            timeLeft -= Time.deltaTime;
        }

        SetInteractable(true);
        cooldownText.gameObject.SetActive(false);
        isOnCooldown = false;
    }
}
