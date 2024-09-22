using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class ConfigKeys
{
    public const string CURRENT_PET_KEY = "currentPet";
    public const string FUNGALS_KEY = "fungals";
    public const string LEVEL_KEY = "level";
    public const string EXPERIENCE_KEY = "experience";
    public const string INVENTORY_KEY = "inventory";
    public const string HUNGER_KEY = "hunger";
    public const string NAME_KEY = "name";
    public const string PARTNER_KEY = "partner";

    // stats
    public const string STATS_KEY = "stats";
    public const string BALANCE_KEY = "balance";
    public const string STAMINA_KEY = "stamina";
    public const string SPEED_KEY = "speed";
    public const string POWER_KEY = "power";
}

public static class SceneParameters
{
    public static int FungalIndex = 0;
}

// handles persistent data across the game and provides an API to the save data
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Developer Options")]
    [SerializeField] private bool resetDataOnAwake;

    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("Debug")]
    [SerializeField] private List<FungalModel> fungals = new List<FungalModel>();
    [SerializeField] private List<ItemInstance> inventory = new List<ItemInstance>();
    [SerializeField] private string saveDataPath;

    public GameData GameData => gameData;

    public JObject JsonFile { get; private set; }
    public List<FungalModel> Fungals => fungals;
    public List<ItemInstance> Inventory => inventory;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        saveDataPath = $"{Application.persistentDataPath}/data.json";

        if (Application.isEditor && resetDataOnAwake) ResetData();

        if (!File.Exists(saveDataPath)) JsonFile = new JObject();
        else
        {
            try
            {
                var configFile = File.ReadAllText(saveDataPath);
                JsonFile = JObject.Parse(configFile);
            }
            catch
            {
                JsonFile = new JObject();
            }
        }

        RunMigrations();
        UnpackJsonFile();
    }
    #region Protected Methods

    public void AddToInventory(ItemInstance item)
    {
        if (inventory.Count >= 8) return;
        Debug.Log($"adding item {item.Data.name} {JsonFile}");
        inventory.Add(item);
        (JsonFile[ConfigKeys.INVENTORY_KEY] as JArray).Add(item.Data.name);
        File.WriteAllText(saveDataPath, JsonFile.ToString());
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
        }
    }

    public void AddFungal(FungalModel fungal)
    {
        Debug.Log($"adding fungal {fungal.Data.name} {JsonFile}");
        fungals.Add(fungal);
        if (JsonFile.ContainsKey(ConfigKeys.FUNGALS_KEY) && JsonFile[ConfigKeys.FUNGALS_KEY] is JArray fungalArray)
        {
            fungalArray.Add(fungal.Json);
        }
        else
        {
            JsonFile[ConfigKeys.FUNGALS_KEY] = new JArray { fungal.Json };
        }
        SaveData();
    }

    public FungalModel GetPartner()
    {
        var partnerId = JsonFile[ConfigKeys.PARTNER_KEY]?.ToString();
        var partner = fungals.Find(x => x.Data.name == partnerId);
        return partner;
    }

    public void SetPartner(FungalController fungal)
    {
        JsonFile[ConfigKeys.PARTNER_KEY] = fungal ? fungal.Model.name : null;
        SaveData();
    }

    private void OnFungalDataChanged(FungalModel fungal)
    {
        JsonFile[ConfigKeys.FUNGALS_KEY][fungal.Index] = fungal.Json;
        SaveData();
    }

    protected virtual void OnCurrentPetChanged(FungalModel pet)
    {

    }

    public void ResetData()
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

        if (JsonFile.ContainsKey(ConfigKeys.FUNGALS_KEY) && JsonFile[ConfigKeys.FUNGALS_KEY] is JArray fungalArray)
        {
            var i = 0;
            foreach (JObject fungalObject in fungalArray)
            {
                var fungal = ScriptableObject.CreateInstance<FungalModel>();
                var fungalData = gameData.Fungals.FirstOrDefault(fungal => fungal.Id == fungalObject["name"].ToString());
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

    public void SaveData()
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
        AddStatsToFungals();
    }

    // May 27, 2023
    private void MigratePetFieldToObject()
    {
        var petName = JsonFile[ConfigKeys.CURRENT_PET_KEY];
        if (petName?.Type == JTokenType.String)
        {
            Debug.Log($"running {nameof(MigratePetFieldToObject)}");

            var petData = GameData.Fungals.FirstOrDefault(pet => pet.Id == petName.ToString());

            JsonFile[ConfigKeys.CURRENT_PET_KEY] = new JObject
            {
                [ConfigKeys.NAME_KEY] = petData.name,
                [ConfigKeys.HUNGER_KEY] = 100,
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

            currentPetObject[ConfigKeys.LEVEL_KEY] = JsonFile[ConfigKeys.LEVEL_KEY];
            currentPetObject[ConfigKeys.EXPERIENCE_KEY] = JsonFile[ConfigKeys.EXPERIENCE_KEY];

            var fungalsArray = new JArray
            {
                currentPetObject
            };

            JsonFile[ConfigKeys.FUNGALS_KEY] = fungalsArray;

            JsonFile.Remove(ConfigKeys.CURRENT_PET_KEY);
            JsonFile.Remove(ConfigKeys.LEVEL_KEY);
            JsonFile.Remove(ConfigKeys.EXPERIENCE_KEY);
            SaveData();
        }
    }

    // Jun 6, 2023
    private void AddStatsToFungals()
    {
        if (JsonFile[ConfigKeys.FUNGALS_KEY] is JArray fungals)
        {
            for (var i = 0; i < fungals.Count; i++)
            {
                if (JsonFile[ConfigKeys.FUNGALS_KEY][i][ConfigKeys.STATS_KEY] is JObject) continue;

                JsonFile[ConfigKeys.FUNGALS_KEY][i][ConfigKeys.STATS_KEY] = new JObject
                {
                    [ConfigKeys.BALANCE_KEY] = 0f,
                    [ConfigKeys.SPEED_KEY] = 0f,
                    [ConfigKeys.STAMINA_KEY] = 0f,
                    [ConfigKeys.POWER_KEY] = 0f,
                }; ;
            }

            SaveData();
        }
       
    }
    #endregion
}
