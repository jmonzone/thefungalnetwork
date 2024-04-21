using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private bool resetDataOnAwake;

    [Header("Game Data")]
    [SerializeField] private GameData data;

    [Header("Debug")]
    [SerializeField] private Pet pet;
    [SerializeField] private string saveDataPath;

    private JObject saveData;

    protected virtual void Awake()
    {
        saveDataPath = $"{Application.persistentDataPath}/data.json";

        if (Application.isEditor && resetDataOnAwake) ResetData();
        else LoadData();
    }

    #region Protected Methods
    protected GameData Data => data;

    protected Pet CurrentPet
    {
        get => pet;
        set
        {
            pet = value;
            SaveData("currentPet", pet.Name);
        }
    }

    protected void ResetData()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }

        saveData = new JObject();
    }
    #endregion Protected Methods

    #region Private Methods
    private void LoadData()
    {
        if (File.Exists(saveDataPath))
        {
            var configFile = File.ReadAllText(saveDataPath);
            saveData = JObject.Parse(configFile);

            pet = data.Pets.FirstOrDefault(pet => pet.Name == saveData["currentPet"].ToString());
        }
        else
        {
            saveData = new JObject();
        }
    }

    private void SaveData(string key, string value)
    {
        saveData[key] = value;
        File.WriteAllText(saveDataPath, saveData.ToString());
    }
    #endregion Private Methods
}
