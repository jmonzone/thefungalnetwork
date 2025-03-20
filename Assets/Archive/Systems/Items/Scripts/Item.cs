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
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemTags itemTags;

    public Sprite Sprite => sprite;
    public GameObject ItemPrefab => itemPrefab;

    public bool HasAllTags(ItemTags requiredTags)
    {
        return (itemTags & requiredTags) == requiredTags;
    }

    public bool HasOneOfTag(ItemTags requiredTags)
    {
        return (itemTags & requiredTags) != 0;
    }
}
