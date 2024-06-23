using System.Collections.Generic;
using UnityEngine;

public class FishManager : ObjectPoolManager<FishController>
{
    [SerializeField] private FishData defaultFish;
    [SerializeField] private FishController treasure;

    protected override List<FishController> Prefabs => new List<FishController>
    {
        defaultFish.Prefab,
        treasure,
    };

    protected override ObjectPool<FishController> GetTargetPool(Dictionary<FishController, ObjectPool<FishController>> pools)
    {
        var isTreasure = Random.Range(0f, 1f) > 0.9;
        return pools[isTreasure ? treasure : defaultFish.Prefab];
    }

    protected override void OnInstantiate(FishController fish)
    {
        fish.Initialize(defaultFish, spawnPosition.Bounds);
    }

    protected override void OnSpawn(FishController obj)
    {
    }
}

