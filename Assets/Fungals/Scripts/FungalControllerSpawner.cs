using UnityEngine;

public class FungalControllerSpawner : MonoBehaviour
{
    [SerializeField] private FungalController prefab;

    public FungalController SpawnFungal(FungalModel fungal, Vector3 spawnPosition)
    {
        var fungalController = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        fungalController.Initialize(fungal);
        fungalController.transform.forward = Utility.RandomXZVector;
        return fungalController;
    }
}
