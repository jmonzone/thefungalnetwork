using UnityEngine;
using UnityEngine.Events;

public class FungalControllerSpawner : MonoBehaviour
{
    [SerializeField] private FungalController prefab;

    public event UnityAction<FungalController> OnFungalSpawned;

    public FungalController SpawnFungal(FungalModel fungal, Vector3 spawnPosition)
    {
        var fungalController = Instantiate(prefab, spawnPosition, Quaternion.identity);
        fungalController.Initialize(fungal);
        fungalController.transform.forward = Utility.RandomXZVector;
        OnFungalSpawned?.Invoke(fungalController);
        return fungalController;
    }
}
