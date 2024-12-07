using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class FungalInventory : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private GameData gameData;
    [SerializeField] private List<FungalModel> fungals;
    public List<FungalModel> Fungals => fungals;

    public event UnityAction OnFungalsUpdated;

    private const string FUNGALS_KEY = "fungals";

    public void Initialize()
    {
        fungals = new List<FungalModel>();
        if (localData.JsonFile.ContainsKey(FUNGALS_KEY) && localData.JsonFile[FUNGALS_KEY] is JArray fungalArray)
        {
            foreach (JObject fungalJson in fungalArray)
            {
                var fungal = CreateInstance<FungalModel>();
                var fungalData = gameData.Fungals.FirstOrDefault(fungal => fungal.Id == fungalJson["name"].ToString());
                fungal.Initialize(fungalData, fungalJson);
                AddFungalToList(fungal);
            }
        }
    }

    public void SaveData()
    {
        var fungalJson = new JArray();

        foreach (var fungal in Fungals)
        {
            fungalJson.Add(fungal.Json);
        }

        localData.SaveData(FUNGALS_KEY, fungalJson);

    }

    private void AddFungalToList(FungalModel fungal)
    {
        fungals.Add(fungal);
        OnFungalsUpdated?.Invoke();
    }

    public void AddFungal(FungalModel fungal)
    {
        AddFungalToList(fungal);
        SaveData();
    }
}
