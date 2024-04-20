using UnityEngine;

public abstract class Upgrade : ScriptableObject
{
    [SerializeField] private new string name;
    [TextArea]
    [SerializeField] private string description;

    public string Name => name;
    public string Description => description;
    public abstract Sprite Sprite { get; }
}
