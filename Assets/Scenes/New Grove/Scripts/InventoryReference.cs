using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryReference : ScriptableObject
{
    [SerializeField] private int sporeCount = 0;
    [SerializeField] private int initialSporeCount = 124;
    [SerializeField] private bool hasDJTable = false;

    [SerializeField] private List<Item> initialItems;
    [SerializeField] private List<Item> items;

    public int SporeCount => sporeCount;
    public bool HasDJTable => hasDJTable;
    public List<Item> Items => items;

    public event UnityAction<int> OnSporeCountChanged;
    public event UnityAction OnItemSummoned;

    public void Initialize()
    {
        items = new List<Item>(initialItems);
        sporeCount = initialSporeCount;
        hasDJTable = false;
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
        if (item.Name == "DJ Table") hasDJTable = true;
        OnItemSummoned?.Invoke();
    }
}
