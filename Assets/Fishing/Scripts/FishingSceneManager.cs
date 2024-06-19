using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishingSceneManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private UpgradeCollection fishingUpgrades;
    [SerializeField] private FishData defaultFish;

    [Header("Fishing References")]
    [SerializeField] private FungalController fungalController;
    [SerializeField] private Collider fishBounds;
    [SerializeField] private LevelUpUI levelUpUI;
    [SerializeField] private List<LogFlume> logFlumes;
    [SerializeField] private TextPopup textPopup;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private FishingRod fishingRod;
    [SerializeField] private TreasureUI treasureUI;
    [SerializeField] private LayerMask uiLayer;

    [Header("Debug")]
    [SerializeField] private float cooldown = BASE_COOLDOWN;
    [SerializeField] private List<FishData> availableFish;
    [SerializeField] private List<FishController> fishControllers;

    private int flumeIndex = 0;
    private const float BASE_COOLDOWN = 3f;
    private float timer = 0f;
    private int fishCount = 0;
    private Camera mainCamera;

    private Transform RandomSpawnAnchor
    {
        get
        {
            var availableLogFlumes = logFlumes.Where(flume => flume.gameObject.activeSelf).ToList();
            return availableLogFlumes[Random.Range(0, availableLogFlumes.Count)].SpawnAnchor;
        }
    }
    private FishData RandomFish => availableFish[Random.Range(0, availableFish.Count)];

    private void Start()
    {
        //fungalController.Initialize(GameManager.Instance.Fungals[SceneParameters.FungalIndex]);
        //fungalController.SetFish(fishControllers);
        fungalController.Model.OnExperienceChanged += OnExperienceChanged;
        fungalController.Model.OnLevelChanged += OnLevelChanged;
        fungalController.Model.OnLevelUp += OnLevelUp;

        OnLevelChanged(fungalController.Model.Level);
        OnExperienceChanged(fungalController.Model.Experience);

        mainCamera = Camera.main;

        if (GameManager.Instance.JsonFile.ContainsKey("flumes")) flumeIndex = (int)GameManager.Instance.JsonFile["flumes"];
        else flumeIndex = 0;

        UpdateFlumes();

        //fishingRod.OnReeledIn += fish =>
        //{
        //    if (fish.IsTreasure)
        //    {
        //        flumeIndex++;
        //        GameManager.Instance.JsonFile["flumes"] = flumeIndex;
        //        GameManager.Instance.SaveData();

        //        treasureUI.gameObject.SetActive(true);
        //        UpdateFlumes();
        //    }
        //};
    }

    private void UpdateFlumes()
    {
        for (var i = 0; i < flumeIndex + 1 && i < logFlumes.Count; i++)
        {
            var flume = logFlumes[i];
            flume.gameObject.SetActive(true);
        }

        cooldown = BASE_COOLDOWN / (flumeIndex + 1);
    }

    private void Update()
    {
        if (timer > 1 && fishCount < 5)
        {
            SpawnFish();
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime / cooldown;
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            fungalController.Model.Experience = FungalModel.ExperienceAtLevel(fungalController.Model.Level + 1) + 10f;
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

    private void SpawnFish()
    {
        var randomFish = RandomFish;
        var fish = Spawn(randomFish.Prefab.gameObject);
        fish.name = $"{fishCount} {fish.name}";
        fishCount++;

        var fishController = fish.GetComponent<FishController>();

        if (fishController)
        {
            fishController.Initialize(randomFish, fishBounds);
            fishControllers.Add(fishController);
            fishController.OnCaught += OnFishCaught;

            void OnFishCaught()
            {
                Debug.Log("fish caught");
                //if (!fishController.IsTreasure)
                //{
                //    var item = ScriptableObject.CreateInstance<ItemInstance>();
                //    item.Initialize(fishController.Data);
                //    Debug.Log($"item initialized {fishController.Data}");

                //    GameManager.Instance.AddToInventory(item);

                //    var experience = fishController.Data.Experience;
                //    fungalController.Model.Experience += experience;
                //    Debug.Log($"adding experience {experience}");

                //    var position = mainCamera.WorldToScreenPoint(fish.transform.position + Vector3.up);
                //    textPopup.transform.position = position;
                //    textPopup.ShowText($"+{experience}");

                //    Debug.Log($"text popup at {position}");
                //}

                fishCount--;
                fishController.OnCaught -= OnFishCaught;
            }
        }
    }

    public GameObject Spawn(GameObject prefab)
    {
        var spawnPosition = RandomSpawnAnchor.position;
        var go = Instantiate(prefab, spawnPosition, Quaternion.identity);
        var fishController = go.GetComponent<FishController>();
        //fishController.IsCatchable = false;

        //var movement = go.GetComponent<MovementController>();

        //void OnDestinationReached()
        //{
        //    fishController.IsCatchable = true;
        //    movement.Speed = 1f;
        //    movement.normalizeSpeed = true;
        //    movement.SetBounds(fishBounds.bounds);
        //    movement.OnDestinationReached -= OnDestinationReached;
        //}

        //var inWaterPosition = movement.transform.position + Vector3.right * Random.Range(2f, 7f);
        //inWaterPosition.y = 0;
        //movement.Speed = 3f;
        //movement.SetTargetPosition(inWaterPosition);
        //movement.OnDestinationReached += OnDestinationReached;

        return go;
    }

    private void OnExperienceChanged(float experience)
    {
        experienceSlider.value = experience;
    }

    private void OnLevelChanged(int level)
    {
        levelText.text = (level).ToString();
        experienceSlider.minValue = FungalModel.ExperienceAtLevel(level);
        experienceSlider.maxValue = FungalModel.ExperienceAtLevel(level + 1);

        availableFish = new List<FishData>(GameManager.Instance.GameData.Items.OfType<FishData>().Where(fish => fish.LevelRequirement <= level));
    }

    private void OnLevelUp()
    {
        if (fishingUpgrades.OfLevel(fungalController.Model.Level, out Upgrade upgrade))
        {
            levelUpUI.Show(fungalController.Model.Level, upgrade);
        }

        if (flumeIndex < logFlumes.Count && fungalController.Model.Level % 5 == 0)
        {
            Spawn(treasurePrefab);
        }
    }
}
