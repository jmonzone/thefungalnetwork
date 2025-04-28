using TMPro;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameReference minigameReference; // The ScriptableObject with the timer
    [SerializeField] private TextMeshProUGUI timerText; // Assign your UI Text here
    [SerializeField] private FadeCanvasGroup deathUI;

    [Header("Display Settings")]
    [SerializeField] private bool hideWhenZero = true; // Optional: hides text when timer hits 0

    private void Awake()
    {
        minigameReference.OnClientPlayerAdded += MinigameReference_OnClientPlayerAdded;
        enabled = false;
    }

    private void MinigameReference_OnClientPlayerAdded()
    {
        minigameReference.OnClientPlayerAdded -= MinigameReference_OnClientPlayerAdded;

        minigameReference.ClientPlayer.Fungal.Fungal.OnRespawnStart += MinigameReference_OnRespawnStart;
        minigameReference.ClientPlayer.Fungal.Fungal.OnRespawnComplete += MinigameReference_OnRespawnComplete;
        enabled = true;
    }

    private void MinigameReference_OnRespawnComplete()
    {
        StartCoroutine(deathUI.FadeOut());
    }

    private void MinigameReference_OnRespawnStart()
    {
        StartCoroutine(deathUI.FadeIn());
    }

    private void Update()
    {
        // Make sure we have a reference
        if (minigameReference == null || timerText == null)
            return;

        float timeRemaining = minigameReference.ClientPlayer.Fungal.Fungal.RemainingRespawnTime;

        // If you want the UI to disappear when timer is zero
        if (hideWhenZero && timeRemaining <= 0f)
        {
            timerText.gameObject.SetActive(false);
            return;
        }

        // Otherwise show and update the text
        timerText.gameObject.SetActive(true);

        // Display as whole seconds (rounded up)
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }
}
