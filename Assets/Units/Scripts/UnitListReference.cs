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

                    float friendshipPoints = unitJson.Value<float?>("friendshipPoints") ?? 0f;

                    var instance = CreateInstance<UnitInstance>();
                    instance.Initialize(matchingUnit, unitId, friendshipPoints, matchingColorPalette, unitJson);
                    RegisterUnit(instance, false);
                };
            }
        }

        foreach (var unit in units)
        {
            // Get the friends array from the unit's JObject
            if (unit.Json?["friends"] is not JArray friendsArray) continue;

            foreach (var friendIdToken in friendsArray)
            {
                string friendId = friendIdToken?.ToString();
                if (string.IsNullOrEmpty(friendId)) continue;

                // Find the matching UnitInstance in your units list
                var friendUnit = units.Find(u => u.Id == friendId);
                if (friendUnit != null && !unit.Friends.Contains(friendUnit))
                {
                    unit.Friends.Add(friendUnit);
                }
            }
        }

        if (units.Count == 0)
        {
            //todo: use scriptable asset
            var friendship = UnitInstance.GetXPFromLevel(2);
            var partyFrogId = "000000000000000000000000";
            var instance = CreateInstance<UnitInstance>();
            instance.Initialize(unitCollection[0], partyFrogId, friendship, null);
            RegisterUnit(instance, false);
        }

        SaveData();
    }

    public Unit RandomUnitData
    {
        get
        {
            var prioritizedList = unitCollection.Where(unit => !Units.Any(unitInstance => unitInstance.Data == unit)).ToList();

            if (prioritizedList.Count > 0)
            {
                return prioritizedList[UnityEngine.Random.Range(0, prioritizedList.Count)];
            }

            return unitCollection[UnityEngine.Random.Range(0, unitCollection.Count)];
        }
    }

    public UnitInstance FindOrCreateFriend(UnitInstance instance)
    {
        if (instance.Friends.Count > 0)
        {
            return instance.Friends[UnityEngine.Random.Range(0, instance.Friends.Count)];
        }
        else
        {
            var friend = CreateInstance<UnitInstance>();
            friend.Initialize(RandomUnitData, colorPalette: RandomColorPalette);
            instance.Friends.Add(friend);
            friend.Friends.Add(instance);
            RegisterUnit(friend);
            return friend;
        }
    }

    public UnitInstance RegisterUnit(UnitInstance instance, bool saveData = true)
    {
        instance.OnFriendshipPointsChanged += _ => SaveData();
        instance.OnFriendshipLevelChanged += () => OnFungalUpdated?.Invoke();

        units.Add(instance);

        if (saveData) SaveData();
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
                ["friends"] = new JArray(unit.Friends.Select(friend => friend.Id)),
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
