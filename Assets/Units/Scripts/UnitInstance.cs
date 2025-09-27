using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Job
{
    NONE,
    DJ,
    DANCER
}

public enum Skill
{
    FRIENDSHIP,
    DANCE,
}

[CreateAssetMenu]
public class UnitInstance : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Unit unit;

    [SerializeField] private int friendshipLevel;
    [SerializeField] private float friendshipXP;

    [SerializeField] private int danceLevel;
    [SerializeField] private float danceXP;

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

    public bool IsFriends => friendshipLevel > 1;
    public int FriendshipLevel => friendshipLevel;
    public float FriendshipXP => friendshipXP;

    public int DanceLevel => danceLevel;
    public float DanceXP => danceXP;

    public float GetXP(Skill skill) => skill switch
    {
        Skill.FRIENDSHIP => friendshipXP,
        Skill.DANCE => danceXP,
        _ => friendshipXP,
    };


    public int GetLevel(Skill skill) => skill switch
    {
        Skill.FRIENDSHIP => friendshipLevel,
        Skill.DANCE => danceLevel,
        _ => friendshipLevel,
    };

    public float GetMinXP(Skill skill) => GetXPFromLevel(GetLevel(skill));
    public float GetMaxXP(Skill skill) => GetXPFromLevel(GetLevel(skill) + 1);
    public float GetXPUntilNextLevel(Skill skill) => GetMaxXP(skill) - GetXP(skill);


    // Scale existing friend chance based on friendship level
    float minChance = 0.1f;   // minimum chance to pick existing friend at level 0
    float maxChance = 0.5f;   // maximum chance at max level
    public float IntroduceNewFriendRate => Mathf.Lerp(minChance, maxChance, FriendshipLevel / (float)3);

    public List<UnitInstance> Friends => friends;

    public JObject Json => json;

    public event UnityAction<Skill, float> OnXpChanged;
    public event UnityAction<Skill, int> OnLevelChanged;

    public void Initialize(Unit unit, string id = null, float friendshipXP = 0, float danceXP = 0, Element element = Element.NONE, Job job = Job.NONE, ColorPalette colorPalette = null, JObject json = null)
    {
        this.id = string.IsNullOrEmpty(id) ? GenerateMongoLikeId() : id;
        this.unit = unit;
        this.json = json;

        this.friendshipXP = friendshipXP;
        friendshipLevel = GetLevelFromXP(friendshipXP);

        this.danceXP = danceXP;
        danceLevel = GetLevelFromXP(danceXP);

        this.element = element;
        this.job = job;
        this.colorPalette = colorPalette;

        friends = new List<UnitInstance>();
    }

    public UnitInstance Copy()
    {
        var copy = CreateInstance<UnitInstance>();
        copy.Initialize(Data, Id, FriendshipXP, DanceXP, element, job, ColorPalette);
        return copy;
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

    public void IncreaseSkillXP(Skill skill, float value)
    {
        var xp = GetXP(skill);
        SetSkillXP(skill, xp + value);
        OnXpChanged?.Invoke(skill, value);
    }

    private void SetSkillXP(Skill skill, float value)
    {
        switch (skill)
        {
            case Skill.FRIENDSHIP:
                friendshipXP = value;
                UpdateSkillLevel(skill, ref friendshipLevel, friendshipXP);
                break;

            case Skill.DANCE:
                danceXP = value;
                UpdateSkillLevel(skill, ref danceLevel, danceXP);
                break;
        }
    }

    private void UpdateSkillLevel(Skill skill, ref int currentLevel, float xp)
    {
        var previousLevel = currentLevel;
        currentLevel = GetLevelFromXP(xp);

        if (previousLevel != currentLevel)
        {
            OnLevelChanged?.Invoke(skill, currentLevel);
        }
    }


    public int GetLevelFromXP(float xp)
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
