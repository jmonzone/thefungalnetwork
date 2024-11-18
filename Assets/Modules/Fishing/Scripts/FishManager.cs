using System.Collections.Generic;
using UnityEngine;

public class FishManager : ObjectPoolManager<FishController>
{
    [SerializeField] private FishData defaultFish;

    protected override List<FishController> Prefabs => new List<FishController>
    {
        defaultFish.Prefab,
    };

    protected override ObjectPool<FishController> GetTargetPool(Dictionary<FishController, ObjectPool<FishController>> pools)
    {
        return pools[defaultFish.Prefab];
    }

    protected override void OnInstantiate(FishController fish)
    {
        fish.Initialize(defaultFish, spawnPosition.Bounds);
    }

    protected override void OnSpawn(FishController obj)
    {
    }
}

