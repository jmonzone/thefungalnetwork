using System.Collections.Generic;
using GURU;
using GURU.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private GameState initialState;
    [SerializeField] private bool useTimer = false;
    [SerializeField] private float gameplayTime = 1f;

    [Header("Gameplay")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private FishManager fishManager;
    [SerializeField] private FishingRod fishingRod;
    [SerializeField] private TextPopup textPopup;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private TreasureUI treasureUI;
    [SerializeField] private List<LogFlume> logFlumes;
    [SerializeField] private LayerMask uiLayer;

    [Header("Results")]
    [SerializeField] private GameObject resultsUI;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private Button mainMenuButton;

    private float experience;
    private int level;
    private int score = 0;
    private float timer = 0;
    private GameState currentState;
    private int flumeIndex = 0;

    private enum GameState
    {
        MAIN_MENU,
        GAMEPLAY,
        RESULTS
    }

    private void Awake()
    {
        var mainCamera = Camera.main.transform;
        fishingRod.OnFishCaught += fish =>
        {
            if (!fish.IsTreasure)
            {
                var experience = fish.Data.Experience;
                Experience += experience;

                var popup = Instantiate(textPopup, fish.transform.position + Vector3.up, Quaternion.identity);
                popup.transform.forward = mainCamera.forward;
                popup.ShowText($"+{experience}");

                score += 100 + Random.Range(0, 50);
                scoreText.text = score.ToString();
            }
        };

        fishingRod.OnReeledIn += fish =>
        {
            if (fish.IsTreasure)
            {
                flumeIndex++;
                var flume = logFlumes[flumeIndex];

                fishManager.AddSpawnAnchor(flume.SpawnAnchor);
                flume.gameObject.SetActive(true);
                treasureUI.gameObject.SetActive(true);
            }
        };



        Level = 1;
        Experience = ExperienceAtLevel(Level);

        SetGameState(initialState);
    }

    private float Experience
    {
        get => experience;
        set
        {
            experience = value;

            var requiredExperience = ExperienceAtLevel(level + 1);
            if (experience > requiredExperience) LevelUp();
            experienceSlider.value = experience;
        }
    }

    private int Level
    {
        get => level;
        set
        {
            level = value;
            levelText.text = (level).ToString();
            experienceSlider.minValue = ExperienceAtLevel(level);
            experienceSlider.maxValue = ExperienceAtLevel(level + 1);

            fishManager.SetLevel(level);
        }
    }

    private void LevelUp()
    {
        Level++;
        fishManager.LevelUp(level);

        if (flumeIndex < logFlumes.Count && Level % 5 == 0)
        {
            fishManager.Spawn(treasurePrefab);
        }
    }

    private float ExperienceAtLevel(int level)
    {
        float total = 0;
        for (int i = 1; i < level; i++)
        {
            total += Mathf.Floor(i + 300 * Mathf.Pow(2, i / 7.0f));
        }

        return Mathf.Floor(total / 4);
    }

    private void SetGameState(GameState state)
    {
        currentState = state;

        gameplayUI.SetActive(state == GameState.GAMEPLAY);
        resultsUI.SetActive(state == GameState.RESULTS);

        fishManager.gameObject.SetActive(state == GameState.GAMEPLAY);
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
                if (useTimer)
                {
                    timer -= Time.deltaTime / 60;

                    int minutes = (int)timer;
                    int seconds = (int)((timer - minutes) * 60);
                    timerText.text = $"{minutes}:{seconds:D2}";

                    if (timer < 0)
                    {
                        SetGameState(GameState.RESULTS);
                        return;
                    }
                }

                if (Input.GetMouseButtonDown(0) && !IsPointerOverUI) fishingRod.Use();
                if (Input.GetKeyUp(KeyCode.L)) Experience = ExperienceAtLevel(level + 1) + 10f;
                break;
        }
    }

    public bool IsPointerOverUI
    {
        get
        {
            PointerEventData eventData = new(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new();
            EventSystem.current.RaycastAll(eventData, raysastResults);

            for (int index = 0; index < raysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = raysastResults[index];
                var maskContainsLayer = (uiLayer & (1 << curRaysastResult.gameObject.layer)) != 0;

                if (maskContainsLayer) return true;
            }

            return false;
        }
    }
}