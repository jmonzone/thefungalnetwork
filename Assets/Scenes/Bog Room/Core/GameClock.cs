using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameClock : MonoBehaviour
{
    [SerializeField] private MultiplayerArena multiplayerArena;
    [SerializeField] private int minutes = 10;

    public event UnityAction<float> OnCountdown;
    public event UnityAction OnCountdownComplete;

    private TextMeshProUGUI timerText;
    private float remainingTime;
    private bool isRunning;

    private void Awake()
    {
        // Get the TextMeshProUGUI component from the children
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        if (timerText == null)
        {
            Debug.LogError("GameClock: No TextMeshProUGUI found in children.");
        }
    }

    private void OnEnable()
    {
        multiplayerArena.OnAllPlayersSpawned += StartClock;
    }

    private void OnDisable()
    {
        multiplayerArena.OnAllPlayersSpawned -= StartClock;
    }

    private void StartClock()
    {
        StartCountdown(minutes * 60); // Convert minutes to seconds
    }

    public void StartCountdown(float durationInSeconds)
    {
        remainingTime = durationInSeconds;
        isRunning = true;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        // Decrease the remaining time
        remainingTime -= Time.deltaTime;
        OnCountdown?.Invoke(minutes * 60 - remainingTime);

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            OnCountdownComplete?.Invoke();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
