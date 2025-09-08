using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class UnitListReference : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference fungalView;
    [SerializeField] private ViewReference fungalListView;

    [SerializeField] private List<Unit> units;

    [SerializeField] private List<Unit> unitCollection;

    public List<Unit> Units => units;

    private const string UNIT_KEY = "units";

    public event UnityAction OnFungalOpened;
    public event UnityAction<Unit> OnFungalSelected;
    public event UnityAction<Unit> OnUnitSummoned;

    public void Initialize()
    {
        foreach(var unit in unitCollection)
        {
            if (unit.Name == "Fox")
            {
                string json = File.ReadAllText("Assets/Dialogue/dialogue.json");
                JObject root = JObject.Parse(json);
                // Example: load "fox"
                JObject foxData = (JObject)root[unit.Name.ToLower()];
                unit.Initialize(foxData);
            }
        }

        units = new List<Unit>();

        if (localData.JsonFile.ContainsKey(UNIT_KEY))
        {
            foreach (var unit in localData.JsonFile[UNIT_KEY] as JArray)
            {
                if (unit is JObject unitJson)
                {
                    var matchingUnit = unitCollection.Find(item => item.Name == unitJson["name"].ToString());
                    if (matchingUnit)
                    {
                        units.Add(matchingUnit);
                    }
                    else
                    {
                        Debug.LogWarning($"Item {unitJson} not found in game data");
                    }
                };
            }
        }

        if (units.Count == 0)
        {
            AddUnit(unitCollection[0]);
        }
    }

    public Unit GetNewUnit(List<Unit> blackList)
    {
        var available = unitCollection.Where(u => !blackList.Contains(u)).ToList();

        if (available.Count > 0)
        {
            return available[Random.Range(0, available.Count)];
        }

        return unitCollection[Random.Range(0, unitCollection.Count)];
    }


    public void AddUnit(Unit unit)
    {
        if (!unit)
        {
            unit = GetNewUnit(units);
        }

        units.Add(unit);
        OnUnitSummoned?.Invoke(unit);
        SaveData();
    }

    private void SaveData()
    {
        var unitsJson = new JArray();

        foreach (var unit in units)
        {
            unitsJson.Add(new JObject
            {
                ["name"] = unit.Name,
            });
        }

        localData.SaveData(UNIT_KEY, unitsJson);
    }

    public void OpenFungals()
    {
        navigation.Navigate(fungalListView);
        OnFungalOpened?.Invoke();
    }

    public void SelectFungal(Unit unit)
    {
        navigation.Navigate(fungalView);
        OnFungalSelected?.Invoke(unit);
    }
}
