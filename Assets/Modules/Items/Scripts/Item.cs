using System;
using UnityEngine;

[Flags]
public enum ItemTags
{
    None = 0,
    MinorIngredient = 1 << 0,
    MajorIngredient = 1 << 1,
    Shrune = 1 << 2
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemTags itemTags;

    public Sprite Sprite => sprite;
    public GameObject ItemPrefab => itemPrefab;

    public bool HasTags(ItemTags requiredTags)
    {
        return (itemTags & requiredTags) == requiredTags;
    }
}
