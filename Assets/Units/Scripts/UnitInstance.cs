using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Job
{
    NONE,
    DJ
}

[CreateAssetMenu]
public class UnitInstance : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Unit unit;
    [SerializeField] private int friendshipLevel;
    [SerializeField] private float friendshipPoints;
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
    public float FriendshipPoints => friendshipPoints;
    public float MinFP => GetXPFromLevel(friendshipLevel);
    public float MaxFP => GetXPFromLevel(friendshipLevel + 1);
    public float FPUntilNextLevel => MaxFP - friendshipPoints;

    // Scale existing friend chance based on friendship level
    float minChance = 0.1f;   // minimum chance to pick existing friend at level 0
    float maxChance = 0.5f;   // maximum chance at max level
    public float IntroduceNewFriendRate => Mathf.Lerp(minChance, maxChance, FriendshipLevel / (float)3);

    public List<UnitInstance> Friends => friends;

    public JObject Json => json;

    public event UnityAction<float> OnFriendshipPointsChanged;
    public event UnityAction OnFriendshipLevelChanged;

    public void Initialize(Unit unit, string id = null, float friendshipPoints = 0, Element element = Element.NONE, Job job = Job.NONE, ColorPalette colorPalette = null, JObject json = null)
    {
        this.id = string.IsNullOrEmpty(id) ? GenerateMongoLikeId() : id;
        this.unit = unit;
        this.friendshipPoints = friendshipPoints;
        friendshipLevel = GetLevelFromXP(friendshipPoints);
        this.element = element;
        this.job = job;
        this.colorPalette = colorPalette;
        friends = new List<UnitInstance>();
        this.json = json;
    }

    public UnitInstance Copy()
    {
        var copy = CreateInstance<UnitInstance>();
        copy.Initialize(Data, Id, FriendshipPoints, element, job, ColorPalette);
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

    public void IncreaseFriendship(float value)
    {
        SetFriendshipPoints(friendshipPoints + value);
        OnFriendshipPointsChanged?.Invoke(value);
    }

    private void SetFriendshipPoints(float value)
    {
        friendshipPoints = value;

        var previousLevel = friendshipLevel;
        friendshipLevel = GetLevelFromXP(friendshipPoints);

        if (previousLevel != friendshipLevel) OnFriendshipLevelChanged?.Invoke();
    }

    public int GetLevelFromXP(float xp)
    {
        int level = 1;
        double points = 0;

        for (int lvl = 1; lvl <= 120; lvl++) // RuneScape goes to 99/120, you can adjust cap
        {
            points += Math.Floor(lvl + 300 * Math.Pow(2, lvl / 7.0));
            double output = Math.Floor(points / (4 * 10));

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

        return (int)Math.Floor(points / (4 * 10));
    }


}
