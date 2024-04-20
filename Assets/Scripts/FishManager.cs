using System.Collections.Generic;
using System.Linq;
using GURU.Entities;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private UpgradeCollection fishingUpgrades;
    [SerializeField] private FishData defaultFish;

    [Header("References")]
    [SerializeField] private Collider fishBounds;
    [SerializeField] private LevelUpUI levelUpUI;
    [SerializeField] private List<Transform> spawnAnchors;

    [Header("Read Only")]
    [SerializeField] private float cooldown = 3f;
    [SerializeField] private float timer = 0f;
    [SerializeField] private List<FishData> availableFish;

    private Transform RandomSpawnAnchor => spawnAnchors[Random.Range(0, spawnAnchors.Count)];
    private FishData RandomFish => availableFish[Random.Range(0, availableFish.Count)];

    public void AddSpawnAnchor(Transform spawnAnchor)
    {
        spawnAnchors.Add(spawnAnchor);
    }

    public void SetLevel(int level)
    {
        availableFish = new List<FishData>(){ defaultFish };

        availableFish.AddRange(fishingUpgrades
            .OfLevel(level)
            .OfType<NewFishUpgrade>()
            .Select(upgrade => upgrade.Fish));
    }

    public void LevelUp(int level)
    {
        if (fishingUpgrades.OfLevel(level, out Upgrade upgrade))
        {
            levelUpUI.Show(level, upgrade);
        }
    }

    private void Update()
    {
        if (timer > 1)
        {
            SpawnFish();
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime / cooldown;
        }
    }

    private void SpawnFish()
    {
        var randomFish = RandomFish;
        var fish = Spawn(randomFish.Prefab.gameObject);
        var fishController = fish.GetComponent<FishController>();
        if (fishController) fishController.SetData(randomFish);

    }

    public GameObject Spawn(GameObject prefab)
    {
        var spawnPosition = RandomSpawnAnchor.position;
        var go = Instantiate(prefab, spawnPosition, Quaternion.identity);
        var fishController = go.GetComponent<FishController>();
        fishController.IsCatchable = false;

        var movement = go.GetComponent<MovementController>();

        void OnDestinationReached()
        {
            fishController.IsCatchable = true;
            movement.Speed = 1f;
            movement.normalizeSpeed = true;
            movement.SetBounds(fishBounds.bounds);
            movement.OnDestinationReached -= OnDestinationReached;
        }

        var inWaterPosition = movement.transform.position + Vector3.right * Random.Range(2f, 7f);
        inWaterPosition.y = 0;
        movement.Speed = 3f;
        movement.SetTargetPosition(inWaterPosition);
        movement.OnDestinationReached += OnDestinationReached;

        return go;
    }
    
}
