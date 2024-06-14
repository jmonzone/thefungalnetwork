using UnityEngine;

public class FishManager : ObjectPool<FishController>
{
    [SerializeField] private FishData defaultFish;

    protected override void OnInstantiate(FishController fish)
    {
        fish.Initialize(defaultFish, spawnPosition.Bounds);
    }

    protected override void OnSpawn(FishController obj)
    {
    }
}

