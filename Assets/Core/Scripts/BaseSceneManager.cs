using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseSceneManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private bool resetDataOnAwake;

    [Header("Game Data")]
    [SerializeField] private GameData data;

    [Header("Debug")]
    [SerializeField] private float experience = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private PetInstance pet;
    [SerializeField] private List<ItemInstance> inventory = new List<ItemInstance>();
    [SerializeField] private string saveDataPath;

    private JObject saveData;

    protected List<ItemInstance> Inventory => inventory;
    public event UnityAction OnInventoryChanged;

    private const string PET_KEY = "currentPet";
    private const string LEVEL_KEY = "level";
    private const string EXPERIENCE_KEY = "experience";
    private const string INVENTORY_KEY = "inventory";

    protected virtual void Awake()
    {
        saveDataPath = $"{Application.persistentDataPath}/data.json";

        if (Application.isEditor && resetDataOnAwake) ResetData();
        else LoadData();

        if (saveData.ContainsKey(LEVEL_KEY) && int.TryParse(saveData[LEVEL_KEY].ToString(), out int level))
        {
            Level = level;
        }
        else
        {
            Level = 1;
        }

        if (saveData.ContainsKey(EXPERIENCE_KEY) && float.TryParse(saveData[EXPERIENCE_KEY].ToString(), out float experience))
        {
            Experience = experience;
        }
        else
        {
            Experience = 0;
        }
    }

    protected virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.L)) Experience = ExperienceAtLevel(level + 1) + 10f;
    }

    #region Protected Methods
    protected GameData Data => data;

    protected void AddToInventory(ItemInstance item)
    {
        if (inventory.Count >= 8) return;
        Debug.Log($"adding item {item.Data.name} {saveData}");
        inventory.Add(item);
        (saveData[INVENTORY_KEY] as JArray).Add(item.Data.name);
        File.WriteAllText(saveDataPath, saveData.ToString());
        OnInventoryChanged?.Invoke();
    }

    protected void RemoveFromInventory(ItemInstance item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            var jarray = saveData[INVENTORY_KEY] as JArray;

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

            File.WriteAllText(saveDataPath, saveData.ToString());
            OnInventoryChanged?.Invoke();
        }
    }

    protected float Experience
    {
        get => experience;
        set
        {
            experience = value;
            OnExperienceChanged(experience);
            SaveData(EXPERIENCE_KEY, experience);
            var requiredExperience = ExperienceAtLevel(level + 1);
            if (experience > requiredExperience) LevelUp();
        }
    }

    protected int Level
    {
        get => level;
        set
        {
            level = value;
            OnLevelChanged(level);
            SaveData(LEVEL_KEY, level);
        }
    }

    protected PetInstance CurrentPet
    {
        get => pet;
        set
        {
            void SavePetData()
            {
                SaveData(PET_KEY, pet.Json);
            }

            if (pet)
            {
                pet.OnDataChanged -= SavePetData;
            }

            pet = value;

            if (pet)
            {
                pet.OnDataChanged += SavePetData;
            }

            
            OnCurrentPetChanged(pet);
        }
    }

    

    protected virtual void OnCurrentPetChanged(PetInstance pet)
    {

    }

    protected void ResetData()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }

        saveData = new JObject();
    }

    protected virtual void LevelUp()
    {
        Level++;
    }

    protected abstract void OnExperienceChanged(float experience);
    protected abstract void OnLevelChanged(int level);

    protected float ExperienceAtLevel(int level)
    {
        float total = 0;
        for (int i = 1; i < level; i++)
        {
            total += Mathf.Floor(i + 300 * Mathf.Pow(2, i / 7.0f));
        }

        return Mathf.Floor(total / 4);
    }
    #endregion Protected Methods

    #region Private Methods
    private void LoadData()
    {
        if (!File.Exists(saveDataPath)) saveData = new JObject();
        else 
        {
            var configFile = File.ReadAllText(saveDataPath);
            saveData = JObject.Parse(configFile);
        }

        if (saveData.ContainsKey(PET_KEY) && !string.IsNullOrEmpty(saveData[PET_KEY].ToString()))
        {
            CurrentPet = ScriptableObject.CreateInstance<PetInstance>();

            if (saveData[PET_KEY] is JObject json)
            {
                var petData = data.Pets.FirstOrDefault(pet => pet.Name == json["name"].ToString());
                CurrentPet.Initialize(petData, json);

                Debug.Log("json available");
            }
            else
            {
                var petData = data.Pets.FirstOrDefault(pet => pet.Name == saveData[PET_KEY].ToString());
                CurrentPet.Initialize(petData);
                Debug.Log(saveData[PET_KEY]);
                Debug.Log($"name available ");

            }
        }
        else
        {
            saveData[PET_KEY] = "";
        }

        if (saveData.ContainsKey(INVENTORY_KEY))
        {
            foreach (var itemName in saveData[INVENTORY_KEY] as JArray)
            {
                var itemData = data.Items.Find(item => item.Name == itemName.ToString());
                var item = ScriptableObject.CreateInstance<ItemInstance>();
                item.Initialize(itemData);
                item.OnConsumed += () => RemoveFromInventory(item);
                if (itemData) inventory.Add(item);
                else Debug.LogWarning($"Item {itemName} not found in game data");
            }
        }
        else
        {
            saveData[INVENTORY_KEY] = new JArray();
        }

        File.WriteAllText(saveDataPath, saveData.ToString());
    }

    private void SaveData(string key, JToken value)
    {
        saveData[key] = value;
        File.WriteAllText(saveDataPath, saveData.ToString());
    }
    #endregion Private Methods
}
