using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu]
public class LocalData : ScriptableObject
{
    [SerializeField] private ItemInventory inventoryService;
    [SerializeField] private FungalInventory fungalService;
    [SerializeField] private Possession possesionService;
    [SerializeField] private bool resetDataOnAwake;
    [SerializeField] private string saveDataPath;

    private JObject jsonFile;

    public void Initialize()
    {
        saveDataPath = $"{Application.persistentDataPath}/data.json";

        if (Application.isEditor && resetDataOnAwake) ResetData();

        if (!File.Exists(saveDataPath)) jsonFile = new JObject();
        else
        {
            try
            {
                var configFile = File.ReadAllText(saveDataPath);
                jsonFile = JObject.Parse(configFile);
            }
            catch
            {
                jsonFile = new JObject();
            }
        }

        inventoryService.Initialize(jsonFile);
        fungalService.Initialize(jsonFile);
        possesionService.Initialize(jsonFile);
        inventoryService.OnInventoryUpdated += SaveData;
        fungalService.OnFungalsUpdated += SaveData;
        possesionService.OnPossessionChanged += SaveData;
    }


    private void SaveData()
    {
        inventoryService.SaveData(jsonFile);
        fungalService.SaveData(jsonFile);
        possesionService.SaveData(jsonFile);

        File.WriteAllText(saveDataPath, jsonFile.ToString());
    }

    private void ResetData()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }

        jsonFile = new JObject();
    }
}
