using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FishingTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 180f; // 3 minutes (180 seconds)

    private float timeRemaining;
    private bool isRunning = true;

    public event UnityAction OnTimerComplete;

    private void Start()
    {
        timeRemaining = startTime;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;
            OnTimerComplete?.Invoke();
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes}:{seconds:D2}";
    }
}
