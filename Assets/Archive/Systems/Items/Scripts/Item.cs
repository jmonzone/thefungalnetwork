using System;
using UnityEngine;

[Flags]
public enum ItemTags
{
    None = 0,
    Ingredient = 1 << 0,
    Shrune = 1 << 1
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int culturePoints;
    [SerializeField] private int price;
    [SerializeField] private ItemTags itemTags;

    public string Name => name;
    public Sprite Sprite => sprite;
    public string Description => description;
    public GameObject ItemPrefab => itemPrefab;
    public int CulturePoints => culturePoints;
    public int Price => price;

    public bool HasAllTags(ItemTags requiredTags)
    {
        return (itemTags & requiredTags) == requiredTags;
    }

    public bool HasOneOfTag(ItemTags requiredTags)
    {
        return (itemTags & requiredTags) != 0;
    }
}
