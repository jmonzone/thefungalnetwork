using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class UnitListReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private LocalData localData;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference fungalView;
    [SerializeField] private ViewReference fungalListView;
    [SerializeField] private TextAsset textAsset;

    [Header("Collections")]
    [SerializeField] private List<UnitInstance> initialUnits;
    [SerializeField] private Unit playerUnit;
    [SerializeField] private List<Unit> unitCollection;
    [SerializeField] private List<Job> jobCollection;
    [SerializeField] private List<Skill> skillCollection;
    [SerializeField] private List<ColorPalette> colorPalettes;

    [Header("Runtime")]
    [SerializeField] private List<UnitInstance> units;

    public List<UnitInstance> Units => units;
    public List<UnitInstance> Friends => units.Where(unit => unit.IsFriends).ToList();
    public List<ColorPalette> ColorPalettes => colorPalettes;
    public ColorPalette RandomColorPalette => colorPalettes[Random.Range(0, colorPalettes.Count)];

    private const string UNIT_KEY = "units";

    public event UnityAction<UnitInstance> OnFungalSelected;

    public ColorPalette GetColorPaletteByElement(Element element)
    {
        return colorPalettes.Find(p => p?.Element == element);
    }

    public void Initialize()
    {
        foreach(var unit in unitCollection)
        {
            string json = textAsset.text;
            JObject root = JObject.Parse(json);
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

                    var elementId = unitJson.Value<string>("element");
                    var element = System.Enum.TryParse(elementId, ignoreCase: true, out Element elementResult) ? elementResult : Element.NONE;
                    var matchingColorPalette = GetColorPaletteByElement(element);

                    var jobId = unitJson.Value<string>("job");
                    var matchingJob = jobCollection.Find(job => job.Id == jobId);

                    float friendshipXP = unitJson.Value<float?>("friendshipXP") ?? 0f;

                    var instance = CreateInstance<UnitInstance>();
                    instance.Initialize(matchingUnit, unitId, friendshipXP, element, matchingJob, matchingColorPalette, unitJson);

                    var skillsJson = unitJson.Value<JArray>("skills") ?? new JArray();
                    var skills = new List<UnitSkill>();

                    foreach (var skill in skillCollection)
                    {
                        var skillJson = skillsJson
                            .FirstOrDefault(x => x?["id"]?.ToString() == skill.Id);

                        float xp = skillJson?.Value<float?>("xp") ?? 0f;

                        skills.Add(new UnitSkill(instance, skill, xp));
                    }

                    instance.InitializeSkills(skills);

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
            foreach(var unit in initialUnits)
            {
                CopyUnit(unit, false);
            }

            foreach (var unit in initialUnits)
            {
                var matchingUnit = units.Find(x => x.Id == unit.Id);
                matchingUnit.Friends.AddRange(unit.Friends.Select(friend => units.Find(x => x.Id == friend.Id)));
                CopyUnit(unit, false);
            }
        }

        SaveData();
    }

    public UnitInstance CopyUnit(UnitInstance instance, bool saveData = true)
    {
        var copiedUnit = CreateInstance<UnitInstance>();
        copiedUnit.Initialize(instance.Data, instance.Id, instance.FriendshipXP, instance.Element, instance.Job, instance.ColorPalette);

        var skills = new List<UnitSkill>();

        foreach (var skill in skillCollection)
        {
            skills.Add(new UnitSkill(copiedUnit, skill, 0));
        }

        copiedUnit.InitializeSkills(skills);
        return RegisterUnit(copiedUnit, saveData);
    }

    public (Unit unit, Element element) GenerateNewUnit()
    {
        // Skip the player's own unit
        var availableUnits = unitCollection
            .Where(unit => unit != playerUnit)
            .ToList();

        // Safety check: if no available units, fallback to player unit
        if (availableUnits.Count == 0)
            availableUnits.Add(playerUnit);

        // Step 1: pick a unit the instance hasn’t seen yet
        var unseenUnits = availableUnits
            .Where(u => !Units.Any(ui => ui.Data == u))
            .ToList();

        if (unseenUnits.Count > 0)
        {
            var chosenUnit = unseenUnits[Random.Range(0, unseenUnits.Count)];
            TryPickUnseenElementForUnit(chosenUnit, out Element chosenElement);
            return (chosenUnit, chosenElement);
        }

        // Step 2: all units have been seen → pick a unit that still has unseen elements
        var availablePairs = new List<(Unit, Element)>();
        foreach (var u in availableUnits)
        {
            if (TryPickUnseenElementForUnit(u, out Element e))
                availablePairs.Add((u, e));
        }

        if (availablePairs.Count > 0)
            return availablePairs[Random.Range(0, availablePairs.Count)];

        // Step 3: fallback → any unit and any element
        var fallbackUnit = availableUnits[Random.Range(0, availableUnits.Count)];
        var allElements = (Element[])System.Enum.GetValues(typeof(Element));
        var fallbackElement = allElements[Random.Range(0, allElements.Length)];

        return (fallbackUnit, fallbackElement);
    }

    private bool TryPickUnseenElementForUnit(Unit unit, out Element element)
    {
        var usedElements = Units
            .Where(ui => ui.Data == unit)
            .Select(ui => ui.Element)
            .ToHashSet();

        var allElements = (Element[])System.Enum.GetValues(typeof(Element));
        var unseenElements = allElements
            .Where(e => !usedElements.Contains(e) && e != Element.NONE)
            .ToList();

        if (unseenElements.Count > 0)
        {
            element = unseenElements[Random.Range(0, unseenElements.Count)];
            return true;
        }

        // fallback: all elements used → pick any
        element = allElements[Random.Range(0, allElements.Length)];
        return false;
    }

    public bool TryGetFriend(UnitInstance unit, out UnitInstance friend, List<UnitInstance> blacklist)
    {
        friend = null;

        var introduceNewFriend = unit.Friends.Count switch
        {
            0 => 1f,
            1 => 0.66f,
            2 => 0.33f,
            _ => 0f,
        };

        if (unit.Friends.Count < 3 && Random.value < introduceNewFriend)
        {
            friend = CreateNewFriend(unit);
        }
        else
        {
            var availableFriends = unit.Friends.Where(friend => !blacklist.Contains(friend)).ToList();
            if (availableFriends.Count > 0)
            {
                friend = availableFriends[Random.Range(0, availableFriends.Count)];
            }
        }

        return friend;
    }

    private UnitInstance CreateNewFriend(UnitInstance unit)
    {
        var (newUnit, newElement) = GenerateNewUnit();
        var friend = CreateInstance<UnitInstance>();

        var matchingColorPalette = GetColorPaletteByElement(newElement);

        friend.Initialize(newUnit, element: newElement, colorPalette: matchingColorPalette);
        unit.Friends.Add(friend);
        friend.Friends.Add(unit);
        RegisterUnit(friend);
        return friend;
    }

    public UnitInstance RegisterUnit(UnitInstance unit, bool saveData = true)
    {
        // Check if an instance with the same Id already exists
        var existing = units.FirstOrDefault(u => u.Id == unit.Id);
        if (existing != null)
        {
            return existing; // Return the already-registered instance
        }

        float sum = 0f;
        float lastSaveTime = 0f;

        unit.OnXpChanged += value =>
        {
            sum += value;

            if (sum > 10f || Time.time - lastSaveTime > 30f)
            {
                sum = 0;
                lastSaveTime = Time.time;
                SaveData();
            }
        };

        //unit.OnLevelChanged += (skill, level) => OnFungalUpdated?.Invoke();

        units.Add(unit);

        if (saveData) SaveData();

        return unit;
    }


    public void SaveData()
    {
        //Debug.Log("saving data");
        var unitsJson = new JArray();

        foreach (var unit in units)
        {
            var unitJson = new JObject
            {
                ["id"] = unit.Id,
                ["name"] = unit.Data.Name,
                ["friendshipLevel"] = unit.FriendshipLevel,
                ["friendshipXP"] = unit.FriendshipXP,
                ["element"] = unit.Element.ToString().ToLower(),
                ["job"] = unit.Job?.Id.ToString().ToLower() ?? "none",
                ["friends"] = new JArray(unit.Friends.Select(friend => friend.Id)),
            };

            var skillsJson = new JArray();

            foreach(var skill in unit.Skills.Keys)
            {
                var skillJson = new JObject
                {
                    ["id"] = skill.Id,
                    ["level"] = unit.Skills[skill].Level,
                    ["xp"] = unit.Skills[skill].XP,
                };

                skillsJson.Add(skillJson);
            }

            unitJson["skills"] = skillsJson;
            unitsJson.Add(unitJson);
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

    public event UnityAction<UnitController, UnitInstance> OnFriendInvited;

    public void InviteFriend(UnitController unit)
    {
        var friend = CreateNewFriend(unit.Instance);
        OnFriendInvited?.Invoke(unit, friend);
    }
}
