using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class BuildReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private LocalData localData;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Item initialItem;

    [Header("Runtime")]
    [SerializeField] private int culturePoints;
    [SerializeField] private List<BuildInstance> buildInstances;
    [SerializeField] private List<BuildController> buildControllers;

    public int CulturePoints => culturePoints;
    public List<BuildInstance> BuildInstances => buildInstances;
    public List<BuildController> BuildControllers => buildControllers;

    private const string BUILD_KEY = "build";

    public event UnityAction OnBuildUpdated;

    public void Initialize()
    {
        buildControllers = new List<BuildController>();

        try
        {

            buildInstances = new List<BuildInstance>();
            culturePoints = 0;

            if (localData.JsonFile.ContainsKey(BUILD_KEY))
            {
                foreach (var build in localData.JsonFile[BUILD_KEY] as JArray)
                {
                    if (build is JObject buildJson)
                    {
                        var itemData = inventory.Items.Find(item => item.Name == buildJson["name"].ToString());
                        if (itemData)
                        {
                            var buildData = CreateInstance<BuildInstance>();
                            var positionJson = buildJson["position"];
                            var x = (float)positionJson["x"];
                            var y = (float)positionJson["y"];
                            var z = (float)positionJson["z"];
                            var position = new Vector3(x, y, z);
                            buildData.Initialize(itemData, position);
                            buildInstances.Add(buildData);
                            culturePoints += buildData.Item.CulturePoints;
                        }
                        else
                        {
                            Debug.LogWarning($"Item {buildJson} not found in game data");
                        }
                    };
                }
            }
            else
            {
                var buildData = CreateInstance<BuildInstance>();
                var position = new Vector3(-1.25f, 0f, -2.5f);
                buildData.Initialize(initialItem, position);
                buildInstances.Add(buildData);
                culturePoints += buildData.Item.CulturePoints;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void LoadExistingBuild()
    {
        foreach (var build in buildInstances)
        {
            var item = build.Item;
            var buildController = Instantiate(item.ItemPrefab).GetComponent<BuildController>();
            buildController.transform.position = build.Position;
            buildController.Initialize(item);
            buildController.Place();
            buildControllers.Add(buildController);
        }

        OnBuildUpdated?.Invoke();
    }

    private void SaveData()
    {
        var buildJson = new JArray();

        foreach (var build in BuildInstances)
        {
            buildJson.Add(new JObject
            {
                ["name"] = build.Item.Name,
                ["position"] = new JObject
                {
                    ["x"] = build.Position.x,
                    ["y"] = build.Position.y,
                    ["z"] = build.Position.z,
                },
            });
        }

        localData.SaveData(BUILD_KEY, buildJson);
    }

    public void AddBuild(BuildController buildController)
    {
        buildControllers.Add(buildController);

        var buildData = CreateInstance<BuildInstance>();
        buildData.Initialize(buildController.Item, buildController.transform.position);
        buildInstances.Add(buildData);
        culturePoints += buildController.Item.CulturePoints;
        SaveData();

        OnBuildUpdated?.Invoke();
    }

    public void RemoveBuild(BuildController buildController)
    {
        buildControllers.Remove(buildController);

        var build = buildInstances.Find(build => build.Item == buildController.Item);
        buildInstances.Remove(build);
        culturePoints -= buildController.Item.CulturePoints;
        SaveData();

        OnBuildUpdated?.Invoke();
    }

    public bool Contains(Item item)
    {
        return buildInstances.Find(build => build.Item == item);
    }

    public List<BuildController> FindBuildControllersWhere(Item item)
    {
        return buildControllers.Where(build => build.Item == item).ToList();
    }
}
