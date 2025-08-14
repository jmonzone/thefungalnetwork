using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class UnitListReference : UIReference
{
    [SerializeField] private LocalData localData;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private List<Unit> units;

    [SerializeField] private List<Unit> unitCollection;
    [SerializeField] private Unit summonedUnit;

    public List<Unit> Units => units;

    private const string UNIT_KEY = "units";

    public event UnityAction<Unit> OnUnitSummoned;

    public void Initialize()
    {
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
    }

    public void Summon()
    {
        if (inventory.SporeCount >= 125)
        {
            inventory.DecreaseSporeCount(125);
            units.Add(summonedUnit);
            OnUnitSummoned?.Invoke(summonedUnit);
            Close();

            SaveData();
        }
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
}
