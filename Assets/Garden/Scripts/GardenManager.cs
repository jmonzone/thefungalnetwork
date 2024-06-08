using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GardenManager : MonoBehaviour
{
    [Header("Gameplay References")]
    [SerializeField] private EggSelection eggSelection;
    [SerializeField] private Rigidbody player;

    [Header("Prefabs")]
    [SerializeField] private FungalController fungalControllerPrefab;
    [SerializeField] private EggController eggControllerPrefab;

    [Header("UI References")]
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private Button resetButton;
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private InventoryList inventoryUI;
    [SerializeField] private InventoryList feedUI;

    private List<FungalController> fungalControllers = new List<FungalController>();

    private List<FungalInstance> Fungals => GameManager.Instance.Fungals;
    private List<ItemInstance> Inventory => GameManager.Instance.Inventory;
    private GameData GameData => GameManager.Instance.GameData;

    private enum GameState
    {
        EGG_SELECTION,
        GAMEPLAY,
        PET_INFO
    }

    private void Start()
    {
        if (Fungals.Count > 0)
        {
            SpawnFungals();
            SetCurrentState(GameState.GAMEPLAY);
        }
        else
        {
            eggSelection.OnEggSelected += egg => OnEggHatched(egg);
            eggSelection.SetPets(GameData.Fungals.GetRange(0, 3));
            SetCurrentState(GameState.EGG_SELECTION);
        }

        if (Fungals.Count == 1 && Fungals[0].Level >= 10)
        {
            var availableFungals = GameData.Fungals.Where(fungal => fungal != Fungals[0].Data).ToList();
            Debug.Log(availableFungals.Count);
            var randomIndex = Random.Range(0, availableFungals.Count);
            var secondFungal = availableFungals[randomIndex];
            Debug.Log(secondFungal.name);
            SpawnEgg(secondFungal);
        }

        resetButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ResetData();
            SceneManager.LoadScene(0);
        });

        void UpdateInventory()
        {
            inventoryUI.SetInventory(Inventory);
            feedUI.SetInventory(Inventory);
        }

        GameManager.Instance.OnInventoryChanged += UpdateInventory;
        UpdateInventory();

        gameplayCanvas.SetActive(true);
    }

    private void Update()
    {
        HandleProximityInteractions();
    }

    private const float MINIMUM_PROXIMITY_DISTANCE = 4f;

    private void HandleProximityInteractions()
    {
        EntityController closestEntity = null;
        float closestDistance = MINIMUM_PROXIMITY_DISTANCE;

        var colliders = Physics.OverlapSphere(player.transform.position, MINIMUM_PROXIMITY_DISTANCE);

        foreach(var collider in colliders)
        {
            var entity = collider.GetComponentInParent<EntityController>();
            if (entity && entity.HasInteraction)
            {
                var distance = Vector3.Distance(player.transform.position, entity.transform.position);
                if (distance < MINIMUM_PROXIMITY_DISTANCE && distance < closestDistance)
                {
                    closestEntity = entity;
                    closestDistance = distance;
                }
            }
        }

        if (closestEntity)
        {
            controlPanel.SetProximityAction(closestEntity);
        }
        else
        {
            controlPanel.SetProximityAction(null);
        }
    }

    private void OnEggHatched(EggController egg)
    {
        var fungal = ScriptableObject.CreateInstance<FungalInstance>();
        fungal.Initialize(egg.Fungal);
        GameManager.Instance.AddFungal(fungal);
        SpawnFungal(fungal, egg.transform.position);
        SetCurrentState(GameState.GAMEPLAY);
    }

    private void SpawnEgg(Pet fungal)
    {
        var randomPosition = (Vector3)Random.insideUnitCircle.normalized * 4;
        randomPosition.z = Mathf.Abs(randomPosition.y);
        randomPosition.y = 1;

        var eggController = Instantiate(eggControllerPrefab, randomPosition, Quaternion.identity);
        eggController.Initialize(fungal);
        eggController.OnHatch += () => OnEggHatched(eggController);
    }

    private void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        foreach(var fungal in Fungals)
        {
            var randomPosition = (Vector3)Random.insideUnitCircle.normalized * Random.Range(3, 6);
            randomPosition.z = Mathf.Abs(randomPosition.y);
            randomPosition.y = 0;

            SpawnFungal(fungal, randomPosition);
        }
    }

    private void SpawnFungal(FungalInstance fungal, Vector3 spawnPosition)
    {

        var fungalController = Instantiate(fungalControllerPrefab, spawnPosition, Quaternion.identity);
        fungalController.SetFungal(fungal);
        fungalControllers.Add(fungalController);
    }

    private void SetCurrentState(GameState state)
    {
        eggSelection.gameObject.SetActive(state == GameState.EGG_SELECTION);
    }
}
