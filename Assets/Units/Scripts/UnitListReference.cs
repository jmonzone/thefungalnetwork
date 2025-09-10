using System.Collections.Generic;
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
    [SerializeField] private TextAsset textAsset;

    [SerializeField] private List<UnitInstance> units;

    [SerializeField] private List<Unit> unitCollection;

    public List<UnitInstance> Units => units;
    public List<UnitInstance> Friends => units.Where(unit => unit.IsFriends).ToList();

    private const string UNIT_KEY = "units";

    public event UnityAction<UnitInstance> OnFungalSelected;
    public event UnityAction<UnitInstance> OnUnitSummoned;
    public event UnityAction OnFungalUpdated;

    public void Initialize()
    {
        foreach(var unit in unitCollection)
        {
            string json = textAsset.text;
            JObject root = JObject.Parse(json);
            // Example: load "fox"
            JObject data = (JObject)root[unit.Name.ToLower()];
            unit.Initialize(data);
        }

        units = new List<UnitInstance>();

        if (localData.JsonFile.ContainsKey(UNIT_KEY))
        {
            foreach (var unit in localData.JsonFile[UNIT_KEY] as JArray)
            {
                if (unit is JObject unitJson)
                {
                    var matchingUnit = unitCollection.Find(item => item.Name == unitJson["name"].ToString());
                    if (matchingUnit)
                    {
                        float relationship = unitJson["relationshipPoints"] != null ? unitJson["relationshipPoints"].ToObject<float>() : 0f;
                        RegisterUnit(matchingUnit, relationship, save: false);
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
            var relationship = UnitInstance.GetXPFromLevel(2);
            RegisterUnit(unitCollection[0], relationship, save: false);
        }
    }

    public Unit GetRandomUnit(List<UnitInstance> blackList)
    {
        var available = unitCollection.Where(unit => !blackList.Any(unitInstance => unitInstance.Data == unit)).ToList();

        if (available.Count > 0)
        {
            return available[Random.Range(0, available.Count)];
        }

        return unitCollection[Random.Range(0, unitCollection.Count)];
    }

    public void SummonUnit(Unit unit)
    {
        var instance = RegisterUnit(unit, relationship: 100);
        OnUnitSummoned?.Invoke(instance);
    }

    public UnitInstance RegisterUnit(Unit unit, float relationship, bool save = true)
    {
        var matchingUnit = units.Find(x => x.Data.Name == unit.Name.ToString());
        if (matchingUnit != null) return matchingUnit;

        var instance = new UnitInstance(unit, relationship);

        instance.OnRelationshipChanged += _ => SaveData();
        instance.OnRelationshipLevelChanged += () => OnFungalUpdated?.Invoke();

        units.Add(instance);

        if (save) SaveData();
        return instance;
    }

    public void SaveData()
    {
        var unitsJson = new JArray();

        foreach (var unit in units)
        {
            unitsJson.Add(new JObject
            {
                ["name"] = unit.Data.Name,
                ["relationshipLevel"] = unit.RelationshipLevel,
                ["relationshipPoints"] = unit.RelationshipPoints,
            });
        }

        localData.SaveData(UNIT_KEY, unitsJson);
    }

    public void OpenFungals()
    {
        navigation.Navigate(fungalListView);
    }

    public void SelectFungal(UnitInstance unit)
    {
        OnFungalSelected?.Invoke(unit);
        navigation.Navigate(fungalView);
    }
}
