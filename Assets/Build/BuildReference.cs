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
    [SerializeField] private List<BuildInstance> initialBuilds;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference buildListView;
    [SerializeField] private ViewReference buildPlacementView;
    [SerializeField] private ViewReference buildSelectView;

    [Header("Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [Header("Runtime")]
    [SerializeField] private bool isBuilding;
    [SerializeField] private BuildController currentBuild;
    [SerializeField] private int culturePoints;
    [SerializeField] private List<BuildInstance> buildInstances;
    [SerializeField] private List<BuildController> buildControllers;

    public bool IsBuilding => isBuilding;
    public BuildController CurrentBuild => currentBuild;
    public int CulturePoints => culturePoints;
    public List<BuildInstance> BuildInstances => buildInstances;
    public List<BuildController> BuildControllers => buildControllers;

    private const string BUILD_KEY = "build";

    public event UnityAction OnBuildUpdated;
    public event UnityAction OnBuildStart;
    public event UnityAction OnBuildEnd;

    public void Initialize()
    {

    }

    public void LoadExistingBuild()
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


                            // --- Rotation (safe parse) ---
                            Quaternion rotation = Quaternion.LookRotation(Vector3.back); // default fallback
                            try
                            {
                                if (buildJson.ContainsKey("rotation"))
                                {
                                    var rotationJson = buildJson["rotation"];
                                    var rotX = (float)rotationJson["x"];
                                    var rotY = (float)rotationJson["y"];
                                    var rotZ = (float)rotationJson["z"];
                                    var rotW = (float)rotationJson["w"];
                                    rotation = new Quaternion(rotX, rotY, rotZ, rotW);
                                }
                            }
                            catch (Exception e)
                            {
                                // ignored, will just stay Quaternion.identity
                                Debug.LogError(e);
                            }

                            buildData.Initialize(itemData, position, rotation);
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
                foreach (var build in initialBuilds)
                {
                    buildInstances.Add(build);
                    culturePoints += build.Item.CulturePoints;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        foreach (var build in buildInstances)
        {
            var item = build.Item;
            var buildController = Instantiate(item.ItemPrefab).GetComponent<BuildController>();
            buildController.transform.SetPositionAndRotation(build.Position, build.Rotation);
            buildController.Initialize(item);
            buildController.Place();
            buildControllers.Add(buildController);
        }

        SaveData();

        OnBuildUpdated?.Invoke();
    }

    private void SaveData()
    {
        var buildJson = new JArray();

        foreach (var build in buildControllers)
        {
            buildJson.Add(new JObject
            {
                ["name"] = build.Item.Name,
                ["position"] = new JObject
                {
                    ["x"] = build.transform.position.x,
                    ["y"] = build.transform.position.y,
                    ["z"] = build.transform.position.z,
                },
                ["rotation"] = new JObject
                {
                    ["x"] = build.transform.rotation.x,
                    ["y"] = build.transform.rotation.y,
                    ["z"] = build.transform.rotation.z,
                    ["w"] = build.transform.rotation.w,
                },
            });
        }

        localData.SaveData(BUILD_KEY, buildJson);
    }

    public void ShowBuild()
    {
        isBuilding = true;
        navigation.Navigate(buildListView);
        OnBuildStart?.Invoke();
    }

    public void EndBuild()
    {
        isBuilding = false;
        navigation.GoBack();
        OnBuildEnd?.Invoke();
    }

    public void StartBuildPlacement(Item item)
    {
        currentBuild = Instantiate(item.ItemPrefab).GetComponent<BuildController>();
        currentBuild.Initialize(item);
        currentBuild.StartBuild(groundMask, collisionMask, validMaterial, invalidMaterial);
        navigation.Navigate(buildPlacementView);
        OnBuildStart?.Invoke();
    }

    public void CompleteBuild()
    {
        if (!currentBuild) return;

        buildControllers.Add(currentBuild);

        inventory.DecreaseSporeCount(currentBuild.Item.Price);
        culturePoints += currentBuild.Item.CulturePoints;
        SaveData();

        OnBuildUpdated?.Invoke();
        currentBuild.CompleteBuild();
        currentBuild = null;
        navigation.GoBack();
    }

    public void CancelBuild()
    {
        if (!currentBuild) return;
        Destroy(currentBuild.gameObject);
        currentBuild = null;
        navigation.GoBack();
    }

    public void SelectBuild(BuildController buildController)
    {
        currentBuild = buildController;
        navigation.Navigate(buildSelectView);
    }

    public void CancelSelect()
    {
        if (!currentBuild) return;
        currentBuild = null;
        navigation.GoBack();
        SaveData();
    }

    public void RotateLeft()
    {
        currentBuild.transform.Rotate(Vector3.down, -45f);
    }

    public void RotateRight()
    {
        currentBuild.transform.Rotate(Vector3.down, 45f);
    }

    public void RemoveBuild()
    {
        if (!currentBuild) return;

        buildControllers.Remove(currentBuild);

        culturePoints -= currentBuild.Item.CulturePoints;

        Destroy(currentBuild.gameObject);
        SaveData();

        OnBuildUpdated?.Invoke();

        navigation.GoBack();
    }

    public bool Contains(Item item)
    {
        return buildControllers.Find(build => build.Item == item);
    }

    public List<BuildController> FindBuildControllersWhere(Item item)
    {
        return buildControllers.Where(build => build.Item == item).ToList();
    }
}
