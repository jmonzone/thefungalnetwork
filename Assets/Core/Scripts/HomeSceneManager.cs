using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeSceneManager : BaseSceneManager
{
    [Header("Scene References")]
    [SerializeField] private EggSelection eggSelection;
    [SerializeField] private Button resetButton;
    [SerializeField] private FungalController fungalControllerPrefab;
    [SerializeField] private Rigidbody player;
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private InventoryList inventoryUI;
    [SerializeField] private InventoryList feedUI;

    private List<FungalController> fungalControllers = new List<FungalController>();

    private enum GameState
    {
        EGG_SELECTION,
        GAMEPLAY,
        PET_INFO
    }

    protected override void Start()
    {
        base.Start();

        if (Fungals.Count > 0)
        {
            SpawnFungals();
            SetCurrentState(GameState.GAMEPLAY);
        }
        else
        {
            eggSelection.OnEggSelected += pet => StartCoroutine(OnEggSelected(pet));
            eggSelection.SetPets(GameData.Fungals.GetRange(0, 3));
            SetCurrentState(GameState.EGG_SELECTION);
        }

        resetButton.onClick.AddListener(() =>
        {
            ResetData();
            SceneManager.LoadScene(0);
        });

        void UpdateInventory()
        {
            inventoryUI.SetInventory(Inventory);
            feedUI.SetInventory(Inventory);
        }

        OnInventoryChanged += UpdateInventory;
        UpdateInventory();
    }

    protected override void Update()
    {
        base.Update();
        HandleProximityFungalInteractions();
    }

    private const float MINIMUM_PROXIMITY_DISTANCE = 4f;

    private void HandleProximityFungalInteractions()
    {
        FungalController closestFungal = null;
        float closestDistance = MINIMUM_PROXIMITY_DISTANCE;
        foreach (var fungalController in fungalControllers)
        {
            if (fungalController.FungalInstance)
            {
                var distance = Vector3.Distance(player.transform.position, fungalController.transform.position);
                if (distance < MINIMUM_PROXIMITY_DISTANCE && distance < closestDistance)
                {
                    closestFungal = fungalController;
                    closestDistance = distance;
                }
            }
        }

        if (closestFungal)
        {
            controlPanel.SetClosestFungalInteractions(closestFungal);
        }
        else
        {
            controlPanel.SetClosestFungalInteractions(null);
        }
    }

    private IEnumerator OnEggSelected(Pet petData)
    {
        var fungal = ScriptableObject.CreateInstance<FungalInstance>();
        fungal.Initialize(petData);
        AddFungal(fungal);

        yield return new WaitForSeconds(1f);
        SpawnFungal(fungal);
        SetCurrentState(GameState.GAMEPLAY);
    }

    private void SpawnFungals()
    {
        Debug.Log("Spawning Fungals");
        foreach(var fungal in Fungals)
        {
            SpawnFungal(fungal);
        }
    }

    private void SpawnFungal(FungalInstance fungal)
    {
        var randomPosition = (Vector3)Random.insideUnitCircle.normalized * 3;
        randomPosition.z = randomPosition.y;
        randomPosition.y = 0;

        var fungalController = Instantiate(fungalControllerPrefab, randomPosition, Quaternion.identity);
        fungalController.Initialize(fungal);
        fungalControllers.Add(fungalController);
    }

    private void SetCurrentState(GameState state)
    {
        eggSelection.gameObject.SetActive(state == GameState.EGG_SELECTION);
    }
}
