using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class FungalService : ScriptableObject
{
    [SerializeField] private GameData gameData;
    [SerializeField] private List<FungalModel> fungals;
    public List<FungalModel> Fungals => fungals;

    public event UnityAction OnFungalsUpdated;

    private const string FUNGALS_KEY = "fungals";

    public void Initialize(JObject jsonFile)
    {
        fungals = new List<FungalModel>();
        if (jsonFile.ContainsKey(FUNGALS_KEY) && jsonFile[FUNGALS_KEY] is JArray fungalArray)
        {
            foreach (JObject fungalJson in fungalArray)
            {
                var fungal = CreateInstance<FungalModel>();
                var fungalData = gameData.Fungals.FirstOrDefault(fungal => fungal.Id == fungalJson["name"].ToString());
                fungal.Initialize(fungalData, fungalJson);
                AddFungal(fungal);
            }
        }
    }

    public void SaveData(JObject jsonFile)
    {
        var fungalJson = new JArray();

        foreach (var fungal in Fungals)
        {
            fungalJson.Add(fungal.Json);
        }

        jsonFile[FUNGALS_KEY] = fungalJson;
    }

    public void AddFungal(FungalModel fungal)
    {
        fungals.Add(fungal);
        OnFungalsUpdated?.Invoke();
    }
}
