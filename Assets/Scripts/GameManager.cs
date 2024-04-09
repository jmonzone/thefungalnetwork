using GURU.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private GameState initialState;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private Button playButton;

    [Header("Gameplay")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private Button homeButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private EntityFactory fishFactory;
    [SerializeField] private float gameplayTime = 1f;
    [SerializeField] private FishingRod fishingRod;

    [Header("Results")]
    [SerializeField] private GameObject resultsUI;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private Button mainMenuButton;

    private int score = 0;
    private float timer = 0;
    private GameState currentState;

    private enum GameState
    {
        MAIN_MENU,
        GAMEPLAY,
        RESULTS
    }

    private void Awake()
    {
        playButton.onClick.AddListener(() => SetGameState(GameState.GAMEPLAY));
        homeButton.onClick.AddListener(() => SetGameState(GameState.MAIN_MENU));
        mainMenuButton.onClick.AddListener(() => SetGameState(GameState.MAIN_MENU));

        fishFactory.OnEntityCreated += fish =>
        {
            var movement = fish.GetComponent<MovementController>();
            var direction = Vector3.back;
            movement.SetDirection(direction);
            movement.transform.forward = direction;
        };

        fishingRod.OnFishCaught += () =>
        {
            score += 100 + Random.Range(0, 50);
            scoreText.text = score.ToString();
        };

        SetGameState(initialState);
    }

    private void SetGameState(GameState state)
    {
        currentState = state;

        mainMenuUI.SetActive(state == GameState.MAIN_MENU);
        gameplayUI.SetActive(state == GameState.GAMEPLAY);
        resultsUI.SetActive(state == GameState.RESULTS);

        fishFactory.gameObject.SetActive(state == GameState.GAMEPLAY);
        fishingRod.gameObject.SetActive(state == GameState.GAMEPLAY);

        switch (state)
        {
            case GameState.GAMEPLAY:
                timer = gameplayTime;
                score = 0;
                break;
            case GameState.RESULTS:
                resultsText.text = score.ToString();
                break;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.GAMEPLAY:
                timer -= Time.deltaTime / 60;

                int minutes = (int)timer;
                int seconds = (int)((timer  - minutes) * 60);
                timerText.text = $"{minutes}:{seconds:D2}";

                if (timer < 0) SetGameState(GameState.RESULTS);
                else if (Input.GetMouseButtonUp(0)) fishingRod.Use();
                break;
        }
    }
}
