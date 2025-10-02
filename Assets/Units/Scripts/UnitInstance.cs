using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    public event UnityAction<float> OnXpChanged;

    public void Initialize(Unit unit, string id = null, float friendshipXP = 0, Element element = Element.NONE, Job job = null, ColorPalette colorPalette = null, JObject json = null)
    {
        this.id = string.IsNullOrEmpty(id) ? GenerateMongoLikeId() : id;
        this.unit = unit;
        this.json = json;

        this.friendshipXP = friendshipXP;
        friendshipLevel = UnitSkill.GetLevelFromXP(friendshipXP);

        this.element = element;
        this.job = job;
        this.colorPalette = colorPalette;

        friends = new List<UnitInstance>();
    }

    public void InitializeSkills(List<UnitSkill> skills)
    {
        this.skills = skills;
        Skills = new Dictionary<Skill, UnitSkill>();
        foreach (var skill in this.skills)
        {
            Skills.Add(skill.Skill, skill);
            skill.OnXpChanged += value => OnXpChanged?.Invoke(value);
        }
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
