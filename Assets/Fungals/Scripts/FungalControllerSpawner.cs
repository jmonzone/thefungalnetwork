using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FungalControllerSpawner : MonoBehaviour
{
    [SerializeField] private FungalController prefab;
    [SerializeField] private List<FungalModel> fungals = new List<FungalModel>();

    public List<FungalModel> Fungals => fungals;

    public event UnityAction<FungalController> OnFungalSpawned;

    public FungalController SpawnFungal(FungalModel fungal, Vector3 spawnPosition)
    {
        var fungalController = Instantiate(prefab, spawnPosition, Quaternion.identity);
        fungalController.Initialize(fungal);
        fungalController.transform.forward = Utility.RandomXZVector;
        fungals.Add(fungal);
        OnFungalSpawned?.Invoke(fungalController);
        return fungalController;
    }
}
