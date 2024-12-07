using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroveManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private Possession possesionService;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;
    [SerializeField] private EggController eggControllerPrefab;
    [SerializeField] private Controllable avatar;

    [Header("Position References")]
    [SerializeField] private Transform rabbitHolePosition;
    [SerializeField] private Collider fungalBounds;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Controller controller;

    public List<FungalController> FungalControllers { get; private set; } = new List<FungalController>();

    public event UnityAction OnPlayerSpawned;
    public event UnityAction<FungalController> OnFungalSpawned;

    private void Start()
    {
        if (fungalInventory.Fungals.Count == 0)
        {
            var randomIndex = Random.Range(0, fungalCollection.Data.Count);
            var randomFungal = fungalCollection.Data[randomIndex];
            SpawnEgg(randomFungal);
        }

        SpawnFungals();
        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
        Debug.Log("spawning player");

        var partner = possesionService.Fungal;
        var targetFungal = FungalControllers.Find(fungal => fungal.Model == partner);
        if (targetFungal)
        {
            Debug.Log("Setting fungal controller");
            targetFungal.transform.position = rabbitHolePosition.position;
            controller.SetController(targetFungal.Controllable);
        }
        else
        {
            Debug.Log("Setting player controller");
            avatar.transform.position = rabbitHolePosition.position;
            controller.SetController(avatar);
        }

        OnPlayerSpawned?.Invoke();
    }

    private void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        foreach (var fungal in fungalInventory.Fungals)
        {
            var randomPosition = fungalBounds.GetRandomXZPosition();
            SpawnFungal(fungal, randomPosition);
        }
    }

    private void SpawnEgg(FungalData fungal)
    {
        var randomPosition = (Vector3)Random.insideUnitCircle.normalized * 4;
        randomPosition.z = Mathf.Abs(randomPosition.y);
        randomPosition.y = 1;

        var eggController = Instantiate(eggControllerPrefab, randomPosition, Quaternion.identity);
        eggController.Initialize(fungal);
        eggController.OnHatch += () => OnEggHatched(eggController);
    }

    private void OnEggHatched(EggController egg)
    {
        var fungal = ScriptableObject.CreateInstance<FungalModel>();
        fungal.Initialize(egg.Fungal);
        fungalInventory.AddFungal(fungal);
        SpawnFungal(fungal, egg.transform.position);
    }

    private void SpawnFungal(FungalModel fungal, Vector3 position)
    {
        var fungalController = fungalControllerSpawner.SpawnFungal(fungal, position);
        OnFungalSpawned?.Invoke(fungalController);
        FungalControllers.Add(fungalController);
        fungalController.Controllable.Movement.SetBounds(fungalBounds);
    }
}
