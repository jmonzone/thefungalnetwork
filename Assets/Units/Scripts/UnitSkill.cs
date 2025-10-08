using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMilestone
{
    public string Label { get; }
    public Sprite Sprite { get; }
    public string Description { get; }
}

[Serializable]
public class UnitSkill
{
    [SerializeField] private UnitInstance unit;
    [SerializeField] private Skill skill;
    [SerializeField] private int level;
    [SerializeField] private float xp;
    [SerializeField] private List<DanceMoveInstance> moves;

    public string Label => skill.Id;
    public Skill Skill => skill;
    public int Level => level;
    public float XP => xp;
    public List<DanceMoveInstance> Moves => moves;
    public List<IMilestone> Milestones { get; private set; }

    public float MinXP => GetXPFromLevel(Level);
    public float MaxXP => GetXPFromLevel(Level + 1);
    public float XPUntilNextLevel => MaxXP - XP;

    public event UnityAction<float> OnXpChanged;
    public event UnityAction<UnitInstance> OnLevelUp;
    public event UnityAction<UnitInstance, DanceMoveInstance, bool> OnMilestoneReached;

    public UnitSkill(UnitInstance unit, Skill skill, float xp)
    {
        this.unit = unit;
        this.skill = skill;
        this.xp = xp;
        level = GetLevelFromXP(xp);

        
        moves = new List<DanceMoveInstance>();

        if (!unit.Data)
        {
            Debug.LogWarning($"UnitSkill: missing unit data {unit.Id}");
            return;
        }

        foreach (var move in unit.Data.Moves)
        {
            if (level >= move.LevelRequirement)
            {
                RegisterMove(move);
            }
        }
    }

    public void IncreaseSkillXP(float value)
    {
        //Debug.Log("increasing skill xp");

        var previousLevel = level;

        xp += value;
        OnXpChanged?.Invoke(value);

        level = GetLevelFromXP(xp);

        if (previousLevel != level)
        {
            Milestones = new List<IMilestone>();

            foreach (var move in unit.Data.Moves)
            {
                if (move.LevelRequirement == level)
                {
                    RegisterMove(move);
                    Milestones.Add(move);
                }
            }

            OnLevelUp?.Invoke(unit);
        }
    }

    private void RegisterMove(DanceMove move)
    {
        var moveInstance = ScriptableObject.CreateInstance<DanceMoveInstance>();
        moveInstance.Initialize(move, this);
        moveInstance.OnUpgrade += () => OnMilestoneReached?.Invoke(unit, moveInstance, true);
        moves.Add(moveInstance);
        OnMilestoneReached?.Invoke(unit, moveInstance, false);
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
