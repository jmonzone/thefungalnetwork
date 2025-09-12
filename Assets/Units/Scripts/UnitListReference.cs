using System;
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
    [SerializeField] private List<ColorPalette> colorPalettes;

    public List<UnitInstance> Units => units;
    public List<UnitInstance> Friends => units.Where(unit => unit.IsFriends).ToList();
    public List<ColorPalette> ColorPalettes => colorPalettes;
    public ColorPalette RandomColorPalette => colorPalettes[UnityEngine.Random.Range(0, colorPalettes.Count)];

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
                    var unitName = unitJson.Value<string>("name");
                    var matchingUnit = unitCollection.Find(u => u.Name == unitName);

                    if (matchingUnit == null)
                    {
                        Debug.LogWarning($"Unit '{unitName}' not found in game data.");
                        continue;
                    }

                    var unitId = unitJson.Value<string>("id");


                    var colorPaletteId = unitJson.Value<string>("colorPalette");
                    var matchingColorPalette = colorPalettes.Find(p => p.Id == colorPaletteId);

                    float relationship = unitJson.Value<float?>("friendshipPoints") ?? 0f;

                    RegisterUnit(unitId, matchingUnit, relationship, matchingColorPalette, save: false);
                };
            }
        }

        if (units.Count == 0)
        {
            var relationship = UnitInstance.GetXPFromLevel(2);
            var partyFrogId = "000000000000000000000000";
            RegisterUnit(partyFrogId, unitCollection[0], relationship, colorPalette: null, save: false);
        }
    }

    public Unit GetRandomUnit(List<UnitInstance> blackList)
    {
        var available = unitCollection.Where(unit => !blackList.Any(unitInstance => unitInstance.Data == unit)).ToList();

        if (available.Count > 0)
        {
            return available[UnityEngine.Random.Range(0, available.Count)];
        }

        return unitCollection[UnityEngine.Random.Range(0, unitCollection.Count)];
    }

    public void SummonUnit(Unit unit)
    {
        var id = GenerateMongoLikeId();
        var instance = RegisterUnit(id, unit, relationship: 100, null);
        OnUnitSummoned?.Invoke(instance);
    }

    public string GenerateMongoLikeId()
    {
        byte[] bytes = new byte[12];

        // 4 bytes: current Unix timestamp
        uint timestamp = (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        BitConverter.GetBytes(timestamp).CopyTo(bytes, 0);

        // 3 bytes: random machine identifier
        var machine = new byte[3];
        new System.Random().NextBytes(machine);
        Array.Copy(machine, 0, bytes, 4, 3);

        // 2 bytes: process id (or random)
        ushort pid = (ushort)UnityEngine.Random.Range(0, ushort.MaxValue);
        BitConverter.GetBytes(pid).CopyTo(bytes, 7);

        // 3 bytes: incrementing counter (random for simplicity)
        var counter = new byte[3];
        new System.Random().NextBytes(counter);
        Array.Copy(counter, 0, bytes, 9, 3);

        // Convert to 24-character hex string
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public UnitInstance RegisterUnit(string id, Unit unit, float relationship, ColorPalette colorPalette = null, bool save = true)
    {
        var matchingInstance = units.Find(x => x.Id == id);
        if (matchingInstance != null) return matchingInstance;

        var instance = new UnitInstance(id, unit, relationship, colorPalette);

        instance.OnFriendshipPointsChanged += _ => SaveData();
        instance.OnFriendshipLevelChanged += () => OnFungalUpdated?.Invoke();

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
                ["id"] = unit.Id,
                ["name"] = unit.Data.Name,
                ["friendshipLevel"] = unit.FriendshipLevel,
                ["friendshipPoints"] = unit.FriendshipPoints,
                ["colorPalette"] = unit.ColorPalette?.Id ?? null,
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
