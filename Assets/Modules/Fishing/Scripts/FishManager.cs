using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishData normalFish;
    [SerializeField] private FishData fastFish;

    [SerializeField] private float frequency;
    [SerializeField] private int maxObjectCount;
    [SerializeField] protected PositionAnchor spawnPosition;
    private float timer;

    private int objectCount;

    private void Awake()
    {
        var fishingSpot = GetComponent<Fishing>();
        fishingSpot.OnFishCaught += () => objectCount--;
    }

    private void Update()
    {
        if (objectCount >= maxObjectCount) return;

        if (timer > frequency)
        {
            var fish = Instantiate(normalFish.Prefab, spawnPosition.Position, Quaternion.identity, transform);
            fish.Initialize(normalFish, spawnPosition.Bounds);
            objectCount++;

            timer = 0;
        }

        timer += Time.deltaTime;
    }

}

