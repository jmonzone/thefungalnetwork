using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FishSpawn
{
    public FishData fishData;
    public float weight; 
}

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private List<FishSpawn> fish;

    [SerializeField] private float frequency;
    [SerializeField] private int maxObjectCount;
    [SerializeField] protected PositionAnchor spawnPosition;

    private float timer;

    private int objectCount;

    public event UnityAction<FishController> OnFishSpawned;

    private void Update()
    {
        if (objectCount >= maxObjectCount) return;

        if (timer > frequency)
        {
            var targetFish = GetFishToSpawn();
            var fishController = Instantiate(targetFish.Prefab, spawnPosition.Position, Quaternion.identity, transform);
            //fishController.Initialize(spawnPosition.Bounds);
            objectCount++;

            OnFishSpawned?.Invoke(fishController);

            // Subscribe to disable event
            //fishController.OnCaught += HandleFishDisabled;

            timer = 0;
        }

        timer += Time.deltaTime;
    }

    private FishData GetFishToSpawn()
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (var _fish in fish)
        {
            totalWeight += _fish.weight;
        }

        // Select a random value based on the total weight
        float randomValue = Random.value * totalWeight;
        float cumulativeWeight = 0f;

        foreach (var _fish in fish)
        {
            cumulativeWeight += _fish.weight;
            if (randomValue <= cumulativeWeight)
            {
                return _fish.fishData;
            }
        }

        return fish[0].fishData; // Default to the first fish type if no match (safety fallback)
    }

    private void HandleFishDisabled()
    {
        objectCount--;
    }
}

