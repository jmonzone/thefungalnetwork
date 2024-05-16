using System.Collections.Generic;
using UnityEngine;

public class InventoryList : MonoBehaviour
{
    private List<InventorySlot> inventorySlots;

    public void SetInventory(List<Item> items)
    {
        inventorySlots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>(includeInactive: true));

        var maxIterations = Mathf.Min(inventorySlots.Count, items.Count);

        for (var i = 0; i < maxIterations; i++)
        {
            inventorySlots[i].SetItem(items[i]);
        }

        for (var i = items.Count; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].SetItem(null);
        }
    }
}
