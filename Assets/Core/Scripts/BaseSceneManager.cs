using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class ConfigKeys
{
    public const string CURRENT_PET_KEY = "currentPet";
    public const string LEVEL_KEY = "level";
    public const string EXPERIENCE_KEY = "experience";
    public const string INVENTORY_KEY = "inventory";
    public const string HUNGER_KEY = "hunger";
    public const string NAME_KEY = "name";
}

public abstract class BaseSceneManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private bool resetDataOnAwake;

    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("Debug")]
    [SerializeField] private List<FungalInstance> fungals = new List<FungalInstance>();
    [SerializeField] private List<ItemInstance> inventory = new List<ItemInstance>();
    [SerializeField] private string saveDataPath;

    protected GameData GameData => gameData;

    public JObject JsonFile { get; private set; }
    protected List<FungalInstance> Fungals => fungals;
    protected List<ItemInstance> Inventory => inventory;

    public event UnityAction OnInventoryChanged;

    protected virtual void Awake()
    {
        saveDataPath = $"{Application.persistentDataPath}/data.json";

        if (Application.isEditor && resetDataOnAwake) ResetData();

        if (!File.Exists(saveDataPath)) JsonFile = new JObject();
        else
        {
            var configFile = File.ReadAllText(saveDataPath);
            JsonFile = JObject.Parse(configFile);
        }

        RunMigrations();
        UnpackJsonFile();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        //if (Input.GetKeyUp(KeyCode.L)) Experience = ExperienceAtLevel(level + 1) + 10f;
    }

    #region Protected Methods

    protected void AddToInventory(ItemInstance item)
    {
        if (inventory.Count >= 8) return;
        Debug.Log($"adding item {item.Data.name} {JsonFile}");
        inventory.Add(item);
        (JsonFile[ConfigKeys.INVENTORY_KEY] as JArray).Add(item.Data.name);
        File.WriteAllText(saveDataPath, JsonFile.ToString());
        OnInventoryChanged?.Invoke();
    }

    protected void RemoveFromInventory(ItemInstance item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            var jarray = JsonFile[ConfigKeys.INVENTORY_KEY] as JArray;

            var itemIndex = -1;

            for (var i = 0; i < jarray.Count; i++)
            {
                if (jarray[i].ToString() == item.Data.name)
                {
                    itemIndex = i;
                    break;
                }
            }
            jarray.RemoveAt(itemIndex);

            SaveData();
            OnInventoryChanged?.Invoke();
        }
    }

    protected void AddFungal(FungalInstance fungal)
    {
        Debug.Log($"adding fungal {fungal.Data.name} {JsonFile}");
        fungals.Add(fungal);
        (JsonFile[ConfigKeys.CURRENT_PET_KEY] as JArray).Add(fungal.Json);
        SaveData();
    }

    private void OnFungalDataChanged(FungalInstance fungal)
    {
        Debug.Log("Update fungal data");
        JsonFile[ConfigKeys.CURRENT_PET_KEY][fungal.Index] = fungal.Json;
        SaveData();
    }

    protected virtual void OnCurrentPetChanged(FungalInstance pet)
    {

    }

    protected void ResetData()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }

        JsonFile = new JObject();
    }
    #endregion Protected Methods

    #region Private Methods
    private void UnpackJsonFile()
    {
        Debug.Log("loading game data");

        if (JsonFile.ContainsKey(ConfigKeys.CURRENT_PET_KEY) && JsonFile[ConfigKeys.CURRENT_PET_KEY] is JArray fungalArray)
        {
            var i = 0;
            foreach (JObject fungalObject in fungalArray)
            {
                var fungal = ScriptableObject.CreateInstance<FungalInstance>();
                var fungalData = gameData.Pets.FirstOrDefault(pet => pet.Name == fungalObject["name"].ToString());
                fungal.Initialize(index: i, fungalData, fungalObject);
                fungal.OnDataChanged += () => OnFungalDataChanged(fungal);
                fungals.Add(fungal);
                i++;
            }
        }

        if (JsonFile.ContainsKey(ConfigKeys.INVENTORY_KEY))
        {
            foreach (var itemName in JsonFile[ConfigKeys.INVENTORY_KEY] as JArray)
            {
                var itemData = gameData.Items.Find(item => item.Name == itemName.ToString());
                var item = ScriptableObject.CreateInstance<ItemInstance>();
                item.Initialize(itemData);
                item.OnConsumed += () => RemoveFromInventory(item);
                if (itemData) inventory.Add(item);
                else Debug.LogWarning($"Item {itemName} not found in game data");
            }
        }
        else
        {
            JsonFile[ConfigKeys.INVENTORY_KEY] = new JArray();
        }

        SaveData();
    }

    protected void SaveData(string key, JToken value)
    {
        JsonFile[key] = value;
        SaveData();
    }

    protected void SaveData()
    {
        File.WriteAllText(saveDataPath, JsonFile.ToString());
    }

    #endregion Private Methods

    #region Migrations
    private void RunMigrations()
    {
        Debug.Log("checking available migrations");
        MigratePetFieldToObject();
        MigratePetObjectToArray();
    }

    // May 27, 2023
    private void MigratePetFieldToObject()
    {
        var petName = JsonFile[ConfigKeys.CURRENT_PET_KEY];
        if (petName?.Type == JTokenType.String)
        {
            Debug.Log($"running {nameof(MigratePetFieldToObject)}");

            var petData = GameData.Pets.FirstOrDefault(pet => pet.Name == petName.ToString());

            JsonFile[ConfigKeys.CURRENT_PET_KEY] = new JObject
            {
                ["name"] = petData.name,
                ["hunger"] = 100,
            };

            SaveData();
        }
    }

    // May 27, 2023
    private void MigratePetObjectToArray()
    {
        var currentPetObject = JsonFile[ConfigKeys.CURRENT_PET_KEY];
        if (currentPetObject?.Type == JTokenType.Object)
        {
            Debug.Log($"running {nameof(MigratePetObjectToArray)}");

            currentPetObject["level"] = JsonFile[ConfigKeys.LEVEL_KEY];
            currentPetObject["experience"] = JsonFile[ConfigKeys.EXPERIENCE_KEY];

            var fungalsArray = new JArray
            {
                currentPetObject
            };

            JsonFile[ConfigKeys.CURRENT_PET_KEY] = fungalsArray;

            JsonFile.Remove(ConfigKeys.LEVEL_KEY);
            JsonFile.Remove(ConfigKeys.EXPERIENCE_KEY);
            SaveData();
        }
    }
    #endregion
}
