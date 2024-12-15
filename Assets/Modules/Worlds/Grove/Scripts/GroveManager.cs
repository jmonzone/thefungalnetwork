using System.Collections.Generic;
using UnityEngine;

public class GroveManager : MonoBehaviour
{
    [SerializeField] private InitialController initialController;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Collider fungalBounds;

    private void Awake()
    {
        initialController.OnControllerInitialized += () =>
        {
            SpawnFungals();
        };
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

    private void SpawnFungal(FungalModel fungal, Vector3 position)
    {
        var fungalController = initialController.SpawnFungal(fungal, position);
        fungalController.Movement.SetBounds(fungalBounds);
    }
}
