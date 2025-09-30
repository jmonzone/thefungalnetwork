using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnitSkill
{
    [SerializeField] private Skill skill;
    [SerializeField] private int level;
    [SerializeField] private float xp;

    public Skill Skill => skill;
    public int Level => level;
    public float XP => xp;

    public float MinXP => GetXPFromLevel(Level);
    public float MaxXP => GetXPFromLevel(Level + 1);
    public float XPUntilNextLevel => MaxXP - XP;

    public event UnityAction OnXpChanged;
    public event UnityAction OnLevelUp;

    public UnitSkill(Skill skill, float xp)
    {
        this.skill = skill;
        this.xp = xp;
        level = GetLevelFromXP(xp);
    }

    public void IncreaseSkillXP(float value)
    {
        Debug.Log("increasing skill xp");

        var previousLevel = level;

        xp += value;
        OnXpChanged?.Invoke();

        level = GetLevelFromXP(xp);

        if (previousLevel != level)
        {
            OnLevelUp?.Invoke();
        }
    }

    public static int GetLevelFromXP(float xp)
    {
        int level = 1;
        double points = 0;

        for (int lvl = 1; lvl <= 120; lvl++) // RuneScape goes to 99/120, you can adjust cap
        {
            points += Math.Floor(lvl + 300 * Math.Pow(2, lvl / 7.0));
            double output = Math.Floor(points / (4));

            if (output > xp)
            {
                level = lvl;
                break;
            }
        }

        return level;
    }

    public static int GetXPFromLevel(int level)
    {
        double points = 0;

        for (int lvl = 1; lvl < level; lvl++)
        {
            points += Math.Floor(lvl + 300 * Math.Pow(2, lvl / 7.0));
        }

        return (int)Math.Floor(points / (4));
    }
}

[CreateAssetMenu]
public class UnitInstance : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Unit unit;

    [SerializeField] private int friendshipLevel;
    [SerializeField] private float friendshipXP;

    [SerializeField] private List<UnitSkill> skills;

    public Dictionary<Skill, UnitSkill> Skills = new Dictionary<Skill, UnitSkill>(); 

    [SerializeField] private Element element;
    [SerializeField] private Job job;
    [SerializeField] private ColorPalette colorPalette;
    [SerializeField] private List<UnitInstance> friends;
    [SerializeField] private JObject json;

    public string Id => id;
    public Unit Data => unit;
    public Element Element => element;
    public Job Job => job;
    public ColorPalette ColorPalette => colorPalette;

    public List<Dialogue> Dialogue => Data.ElementalDialogue[element];
    public Dialogue RandomDialogue => Dialogue[UnityEngine.Random.Range(0, Dialogue.Count)];


    public UnitSkill GetSkill(Skill skill) => Skills[skill];

    public int FriendshipLevel => friendshipLevel;
    public float FriendshipXP => friendshipXP;
    public bool IsFriends => friendshipLevel > 1;

    // Scale existing friend chance based on friendship level
    float minChance = 0.1f;   // minimum chance to pick existing friend at level 0
    float maxChance = 0.5f;   // maximum chance at max level
    public float IntroduceNewFriendRate => Mathf.Lerp(minChance, maxChance, friendshipLevel / (float)3);

    public List<UnitInstance> Friends => friends;

    public JObject Json => json;

    public event UnityAction OnXpChanged;

    public void Initialize(Unit unit, string id = null, float friendshipXP = 0, List<UnitSkill> skills = null, Element element = Element.NONE, Job job = null, ColorPalette colorPalette = null, JObject json = null)
    {
        this.id = string.IsNullOrEmpty(id) ? GenerateMongoLikeId() : id;
        this.unit = unit;
        this.json = json;

        this.friendshipXP = friendshipXP;
        friendshipLevel = UnitSkill.GetLevelFromXP(friendshipXP);

        this.skills = skills ?? new List<UnitSkill>();
        Skills = new Dictionary<Skill, UnitSkill>();
        foreach(var skill in this.skills)
        {
            Skills.Add(skill.Skill, skill);
            skill.OnXpChanged += () => OnXpChanged?.Invoke();
        }

        this.element = element;
        this.job = job;
        this.colorPalette = colorPalette;

        friends = new List<UnitInstance>();
    }

    public static string GenerateMongoLikeId()
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
}
