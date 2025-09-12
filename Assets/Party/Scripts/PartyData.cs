using System;
using System.Collections.Generic;
using UnityEngine;

public enum PartyRequirementType
{
    BUILD,
    CULTURE,
}

[Serializable]
public class PartyRequirementData
{
    [SerializeField] private string label;
    [SerializeField] private PartyRequirementType type;
    [SerializeField] private Item item;

    public string Label => label;
    public PartyRequirementType Type => type;
    public Item Item => item;
}

[CreateAssetMenu]
public class PartyData : ScriptableObject
{
    [SerializeField] private string partyName;
    [SerializeField] private int level;
    [SerializeField] private Sprite sprite;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private float duration;
    [SerializeField] private int culturePoints;
    [SerializeField] private List<PartyRequirementData> requirements;
    [SerializeField] private List<UnitInstance> guests;

    public string Name => partyName;
    public int Level => level;
    public Sprite Sprite => sprite;
    public string Description => description;
    public float Duration => duration;
    public int CulturePoints => culturePoints;
    public List<PartyRequirementData> Requirements => requirements;
    public List<UnitInstance> Guests => guests;

}
