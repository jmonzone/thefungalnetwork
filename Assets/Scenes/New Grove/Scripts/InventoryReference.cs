using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryReference : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private int initialSporeCount = 124;
    [SerializeField] private List<Item> initialItems;

    [Header("Runtime")]
    [SerializeField] private int sporeCount = 0;
    [SerializeField] private List<Item> items;

    public int SporeCount => sporeCount;
    public List<Item> Items => items;

    public event UnityAction<int> OnSporeCountChanged;
    public event UnityAction OnItemSummoned;

    public void Initialize()
    {
        items = new List<Item>(initialItems);
        sporeCount = initialSporeCount;
    }

    public void IncreaseSporeCount(int value = 1)
    {
        sporeCount += value;
        OnSporeCountChanged?.Invoke(sporeCount);
    }

    public void DecreaseSporeCount(int value = 1)
    {
        sporeCount -= value;
        OnSporeCountChanged?.Invoke(sporeCount);
    }

    public void SummonItem(Item item)
    {
        items.Add(item);
        DecreaseSporeCount(item.Price);
        OnItemSummoned?.Invoke();
    }
}
