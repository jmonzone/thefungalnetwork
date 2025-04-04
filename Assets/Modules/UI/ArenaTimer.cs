using UnityEngine;
using TMPro;
using System.Collections;

public class ArenaTimer : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float gameDuration = 150f; // 2:30
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.red;

    private float currentTime;
    private bool isRunning = false;
    private bool isFinalCountdown = false;
    private bool hasEnded = false;

    private float pulseSpeed = 2f;
    private float originalFontSize;

    private void Awake()
    {
        originalFontSize = timerText.fontSize;
    }

    private void OnEnable()
    {
        game.OnGameStart += StartTimer;
    }

    private void OnDisable()
    {
        game.OnGameStart -= StartTimer;
    }

    private void StartTimer()
    {
        currentTime = gameDuration;
        isRunning = true;
        isFinalCountdown = false;
        hasEnded = false;
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0f);

        UpdateTimerUI(currentTime);

        if (currentTime <= 5f && !isFinalCountdown)
        {
            isFinalCountdown = true;
        }

        if (currentTime <= 0f && !hasEnded)
        {
            hasEnded = true;
            isRunning = false;
            StartCoroutine(HandleGameSet());
        }
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(time);

        if (seconds <= 5 && seconds > 0)
        {
            timerText.text = seconds.ToString();

            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            timerText.color = Color.Lerp(startColor, endColor, t);
            timerText.fontSize = Mathf.Lerp(originalFontSize * 2f, originalFontSize * 2.5f, t);
        }
        else if (seconds > 5)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            timerText.text = $"{minutes}:{secs:00}";

            timerText.color = startColor;
            timerText.fontSize = originalFontSize;
        }
    }



    private IEnumerator HandleGameSet()
    {
        timerText.text = "Game Set!";
        timerText.color = endColor;
        timerText.fontSize = originalFontSize * 1.4f;

        game.EndGame();

        yield return new WaitForSeconds(1f);

        timerText.gameObject.SetActive(false);
    }
}
