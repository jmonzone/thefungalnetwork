using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class LocalData : ScriptableObject
{
    [SerializeField] private ItemInventory inventoryService;
    [SerializeField] private FungalInventory fungalService;
    [SerializeField] private Possession possesionService;
    [SerializeField] private bool resetDataOnAwake;
    [SerializeField] private string saveDataPath;

    public JObject JsonFile { get; private set; }

    public event UnityAction OnInitialized;

    public void Initialize()
    {
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

        inventoryService.Initialize(JsonFile);
        fungalService.Initialize(JsonFile);
        possesionService.Initialize(JsonFile);
        OnInitialized?.Invoke();
        inventoryService.OnInventoryUpdated += OnUpdated;
        fungalService.OnFungalsUpdated += OnUpdated;
        possesionService.OnPossessionChanged += OnUpdated;
    }


    private void OnUpdated()
    {
        inventoryService.SaveData(JsonFile);
        fungalService.SaveData(JsonFile);
        possesionService.SaveData(JsonFile);
        File.WriteAllText(saveDataPath, JsonFile.ToString());
    }

    public void SaveData(string key, JToken value)
    {
        JsonFile[key] = value;
        File.WriteAllText(saveDataPath, JsonFile.ToString());
    }

    private void ResetData()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }

        JsonFile = new JObject();
    }
}
