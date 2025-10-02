using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DanceMove : ScriptableObject
{
    [SerializeField] private string label;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string description;
    [SerializeField] private string animationName;
    [SerializeField] private float xp = 5;
    [SerializeField] private int levelRequirement = 1;
    [SerializeField] private List<DanceMoveUpgrade> upgrades;

    public string Label => label;
    public Sprite Sprite => sprite;
    public string Description => description;
    public string AnimationName => animationName;
    public float Xp => xp;
    public int LevelRequirement => levelRequirement;
    public List<DanceMoveUpgrade> Upgrades => upgrades;
}

[Serializable]
public class DanceMoveUpgrade
{
    [SerializeField] private int level;
    [SerializeField] private string description;

    public int Level => level;
    public string Description => description;
}

public class DanceMoveInstance : ScriptableObject
{
    [SerializeField] private DanceMove data;
    [SerializeField] private UnitSkill skill;
    [SerializeField] private int upgradeLevel;

    public DanceMove Data => data;
    public int Loops => upgradeLevel;
    public float Xp => data.Xp * (upgradeLevel + 1);
    public string Label => upgradeLevel > 0 ? data.Label : data.Label + new string('+', upgradeLevel);
    public string Description => upgradeLevel > 0 ? data.Description : data.Upgrades[upgradeLevel].Description;

    public event UnityAction OnUpgrade;

    public void Initialize(DanceMove data, UnitSkill skill)
    {
        this.data = data;
        this.skill = skill;

        foreach(var upgrade in data.Upgrades)
        {
            if (skill.Level >= upgrade.Level)
            {
                upgradeLevel++;
            }
        }

        skill.OnLevelUp += CheckForUpgrades;
    }

    private void CheckForUpgrades()
    {
        foreach (var upgrade in data.Upgrades)
        {
            if (skill.Level == upgrade.Level)
            {
                upgradeLevel++;
                OnUpgrade?.Invoke();
                return;
            }
        }
    }
}