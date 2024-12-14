using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// todo: make separate fungal management script
// this script handles spawning fungals in the garden and eggs if needed
public class GroveManager : MonoBehaviour
{
    [SerializeField] private InitialController initialController;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;
    [SerializeField] private EggController eggControllerPrefab;
    [SerializeField] private Collider fungalBounds;

    private void Awake()
    {
        initialController.OnControllerInitialized += () =>
        {
            SpawnFungals();
        };
    }

    private void Start()
    {
        if (fungalInventory.Fungals.Count == 0)
        {
            var randomIndex = Random.Range(0, fungalCollection.Fungals.Count);
            var randomFungal = fungalCollection.Fungals[randomIndex];
            SpawnEgg(randomFungal);
        }
    }

    private void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        // skip the initialController fungal
        var filteredFungals = new List<FungalModel>(fungalInventory.Fungals);
        filteredFungals.Remove(initialController.InitalFungal);

        foreach (var fungal in filteredFungals)
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
        fungalController.Movement.SetBounds(fungalBounds);
    }
}
