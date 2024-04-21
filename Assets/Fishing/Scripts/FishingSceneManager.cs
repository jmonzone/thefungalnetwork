using System.Collections.Generic;
using System.Linq;
using GURU;
using GURU.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishingSceneManager : BaseSceneManager
{
    [Header("Configuration")]
    [SerializeField] private UpgradeCollection fishingUpgrades;
    [SerializeField] private FishData defaultFish;

    [Header("Fishing References")]
    [SerializeField] private PetController petController;
    [SerializeField] private Collider fishBounds;
    [SerializeField] private LevelUpUI levelUpUI;
    [SerializeField] private List<Transform> spawnAnchors;
    [SerializeField] private List<LogFlume> logFlumes;
    [SerializeField] private TextPopup textPopup;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private FishingRod fishingRod;
    [SerializeField] private TreasureUI treasureUI;
    [SerializeField] private LayerMask uiLayer;

    [Header("Debug")]
    [SerializeField] private List<FishData> availableFish;
    [SerializeField] private List<FishController> fishControllers;

    private int flumeIndex = 0;
    private float experience;
    private int level;
    private float cooldown = 3f;
    private float timer = 0f;
    private int fishCount = 0;
    private Camera mainCamera;

    private Transform RandomSpawnAnchor => spawnAnchors[Random.Range(0, spawnAnchors.Count)];
    private FishData RandomFish => availableFish[Random.Range(0, availableFish.Count)];

    protected override void Awake()
    {
        base.Awake();

        petController.SetPet(CurrentPet);
        petController.SetFish(fishControllers);

        Level = 1;
        Experience = ExperienceAtLevel(Level);

        mainCamera = Camera.main;

        fishingRod.OnReeledIn += fish =>
        {
            if (fish.IsTreasure)
            {
                flumeIndex++;
                var flume = logFlumes[flumeIndex];

                AddSpawnAnchor(flume.SpawnAnchor);
                flume.gameObject.SetActive(true);
                treasureUI.gameObject.SetActive(true);
            }
        };
    }

    private void Update()
    {
        if (timer > 1)
        {
            SpawnFish();
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime / cooldown;
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI) fishingRod.Use();
        if (Input.GetKeyUp(KeyCode.L)) Experience = ExperienceAtLevel(level + 1) + 10f;
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

    public void AddSpawnAnchor(Transform spawnAnchor)
    {
        spawnAnchors.Add(spawnAnchor);
    }

    private void SpawnFish()
    {
        var randomFish = RandomFish;
        var fish = Spawn(randomFish.Prefab.gameObject);
        fish.name = $"{fishCount} {fish.name}";
        fishCount++;

        var fishController = fish.GetComponent<FishController>();

        if (fishController)
        {
            fishController.SetData(randomFish);
            fishControllers.Add(fishController);
            fishController.OnCaught += OnFishCaught;

            void OnFishCaught()
            {
                if (!fishController.IsTreasure)
                {
                    var experience = fishController.Data.Experience;
                    Experience += experience;

                    var position = mainCamera.WorldToScreenPoint(fish.transform.position + Vector3.up);
                    textPopup.transform.position = position;
                    textPopup.ShowText($"+{experience}");
                }

                fishControllers.Remove(fishController);
                fishController.OnCaught -= OnFishCaught;
            }
        }
    }

    public GameObject Spawn(GameObject prefab)
    {
        var spawnPosition = RandomSpawnAnchor.position;
        var go = Instantiate(prefab, spawnPosition, Quaternion.identity);
        var fishController = go.GetComponent<FishController>();
        fishController.IsCatchable = false;

        var movement = go.GetComponent<MovementController>();

        void OnDestinationReached()
        {
            fishController.IsCatchable = true;
            movement.Speed = 1f;
            movement.normalizeSpeed = true;
            movement.SetBounds(fishBounds.bounds);
            movement.OnDestinationReached -= OnDestinationReached;
        }

        var inWaterPosition = movement.transform.position + Vector3.right * Random.Range(2f, 7f);
        inWaterPosition.y = 0;
        movement.Speed = 3f;
        movement.SetTargetPosition(inWaterPosition);
        movement.OnDestinationReached += OnDestinationReached;

        return go;
    }

    private void LevelUp()
    {
        Level++;

        if (fishingUpgrades.OfLevel(level, out Upgrade upgrade))
        {
            levelUpUI.Show(level, upgrade);
        }

        if (flumeIndex < logFlumes.Count && Level % 5 == 0)
        {

            
            Spawn(treasurePrefab);
        }
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

            availableFish = new List<FishData>() { defaultFish };

            availableFish.AddRange(fishingUpgrades
                .OfLevel(level)
                .OfType<NewFishUpgrade>()
                .Select(upgrade => upgrade.Fish));
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
}
