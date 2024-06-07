using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    BALANCE,
    SPEED,
    STAMINA,
    POWER,
}

[Serializable]
public class StatGrowth
{
    [SerializeField] private StatType type;
    [SerializeField] private float growth;

    public StatType Type => type;
    public float Growth => growth;
}

[CreateAssetMenu]
public class FishData : Item
{
    [SerializeField] private FishController prefab;
    [SerializeField] private float experience;
    [SerializeField] private int levelRequirement;
    [SerializeField] private List<StatGrowth> statGrowth;

    public FishController Prefab => prefab;
    public float Experience => experience;
    public int LevelRequirement => levelRequirement;
    public List<StatGrowth> StatGrowth => statGrowth;

}
