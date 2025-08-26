using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class BuildSystem : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Item initialItem;

    [SerializeField] private int culturePoints;
    [SerializeField] private List<BuildData> builds;

    public int CulturePoints => culturePoints;
    public List<BuildData> Builds => builds;

    private const string BUILD_KEY = "build";

    public event UnityAction OnBuildUpdated;
    public event UnityAction OnBuildLoaded;

    public void Initialize()
    {
        try
        {

            builds = new List<BuildData>();
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
                            var buildData = CreateInstance<BuildData>();
                            var positionJson = buildJson["position"];
                            var x = (float)positionJson["x"];
                            var y = (float)positionJson["y"];
                            var z = (float)positionJson["z"];
                            var position = new Vector3(x, y, z);
                            buildData.Initialize(itemData, position);
                            builds.Add(buildData);
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
                var buildData = CreateInstance<BuildData>();
                var position = new Vector3(-1.25f, 0f, -2.5f);
                buildData.Initialize(initialItem, position);
                builds.Add(buildData);
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
        foreach (var build in builds)
        {
            var item = build.Item;
            var buildController = Instantiate(item.ItemPrefab).GetComponent<BuildController>();
            buildController.transform.position = build.Position;
            buildController.Initialize(item);
            buildController.Place();
        }

        OnBuildLoaded?.Invoke();
    }

    private void SaveData()
    {
        var buildJson = new JArray();

        foreach (var build in Builds)
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

    public void AddBuild(Item item, Vector3 position)
    {
        var buildData = CreateInstance<BuildData>();
        buildData.Initialize(item, position);
        builds.Add(buildData);
        culturePoints += item.CulturePoints;
        SaveData();

        OnBuildUpdated?.Invoke();
    }

    public void RemoveBuild(Item item)
    {
        var build = builds.Find(build => build.Item == item);
        builds.Remove(build);
        culturePoints -= item.CulturePoints;
        SaveData();

        OnBuildUpdated?.Invoke();
    }

    public bool Contains(Item item)
    {
        return builds.Find(build => build.Item == item);
    }
}
