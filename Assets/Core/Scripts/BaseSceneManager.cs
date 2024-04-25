using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private bool resetDataOnAwake;

    [Header("Game Data")]
    [SerializeField] private GameData data;

    [Header("Debug")]
    [SerializeField] private float experience = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private Pet pet;
    [SerializeField] private string saveDataPath;

    private JObject saveData;

    private const string PET_KEY = "currentPet";
    private const string LEVEL_KEY = "level";
    private const string EXPERIENCE_KEY = "experience";

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

    protected Pet CurrentPet
    {
        get => pet;
        set
        {
            pet = value;
            SaveData(PET_KEY, pet.Name);
            OnCurrentPetChanged(pet);
        }
    }

    protected virtual void OnCurrentPetChanged(Pet pet)
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
        if (File.Exists(saveDataPath))
        {
            var configFile = File.ReadAllText(saveDataPath);
            saveData = JObject.Parse(configFile);

            if (saveData.ContainsKey(PET_KEY))
            {
                pet = data.Pets.FirstOrDefault(pet => pet.Name == saveData[PET_KEY].ToString());
            }
        }
        else
        {
            saveData = new JObject();
        }
    }

    private void SaveData(string key, JToken value)
    {
        saveData[key] = value;
        File.WriteAllText(saveDataPath, saveData.ToString());
    }
    #endregion Private Methods
}
