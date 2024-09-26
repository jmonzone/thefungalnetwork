using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroveManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalController fungalControllerPrefab;
    [SerializeField] private EggController eggControllerPrefab;

    [Header("Position References")]
    [SerializeField] private Transform rabbitHolePosition;
    [SerializeField] private Collider fungalBounds;

    public List<FungalController> FungalControllers { get; private set; } = new List<FungalController>();

    public event UnityAction OnPlayerSpawned;
    public event UnityAction<FungalController> OnFungalInteracted;

    private void Start()
    {
        if (GameManager.Instance.Fungals.Count == 0) {
            var randomIndex = Random.Range(0, fungalCollection.Data.Count);
            var randomFungal = fungalCollection.Data[randomIndex];
            SpawnEgg(randomFungal);
        }

        SpawnFungals();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var inputManager = GetComponentInChildren<InputManager>();
        var player = GetComponentInChildren<PlayerController>();

        var partner = GameManager.Instance.GetPartner();
        var targetFungal = FungalControllers.Find(fungal => fungal.Model == partner);
        if (targetFungal)
        {
            targetFungal.transform.position = rabbitHolePosition.position;
            inputManager.SetMovementController(targetFungal.Movement);
        }
        else
        {
            player.transform.position = rabbitHolePosition.position;
            inputManager.SetMovementController(player.Movement);
        }

        OnPlayerSpawned?.Invoke();
    }

    private void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        foreach (var fungal in GameManager.Instance.Fungals)
        {
            var randomPosition = fungalBounds.GetRandomXZPosition();
            SpawnFungal(fungal, randomPosition);
        }
    }

    private void SpawnFungal(FungalModel fungal, Vector3 spawnPosition)
    {
        var fungalController = Instantiate(fungalControllerPrefab, spawnPosition, Quaternion.identity);
        fungalController.Initialize(fungal, fungalBounds);
        fungalController.transform.forward = Utility.RandomXZVector;
        FungalControllers.Add(fungalController);
        fungalController.OnInteractionStarted += () => OnFungalInteracted?.Invoke(fungalController);
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
        GameManager.Instance.AddFungal(fungal);
        SpawnFungal(fungal, egg.transform.position);
    }
}
