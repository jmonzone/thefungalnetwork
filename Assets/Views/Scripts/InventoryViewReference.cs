using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryViewReference : ViewReference
{
    [SerializeField] private ItemInventory inventory;

    private Predicate<ItemInstance> cachedFilter;

    public event UnityAction OnInventoryUpdated
    {
        add => inventory.OnInventoryUpdated += value;
        remove => inventory.OnInventoryUpdated -= value;
    }

    public void Open(Predicate<ItemInstance> filter)
    {
        cachedFilter = filter;
        Open();
    }

    public List<ItemInstance> GetFilteredItems(Predicate<ItemInstance> filter = null)
    {
        if (filter == null)
        {
            if (cachedFilter == null)
            {
                return inventory.Items;
            }
            else
            {
                return inventory.Items.FindAll(cachedFilter).ToList();
            }
        }
        else
        {
            return inventory.Items.FindAll(filter).ToList();
        }
    }
}