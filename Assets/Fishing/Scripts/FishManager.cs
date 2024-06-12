using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class FishManager : MonoBehaviour
{
    [SerializeField] private FishData defaultFish;
    [SerializeField] private Collider bounds;

    private void Awake()
    {
        var objectPool = GetComponent<ObjectPool>();
        objectPool.OnInstantiate += obj =>
        {
            var fish = obj.GetComponent<FishController>();
            fish.Initialize(defaultFish, bounds);
        };
    }
}
