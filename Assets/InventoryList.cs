using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryList : MonoBehaviour
{
    private List<InventorySlot> inventorySlots;

    public event UnityAction<ItemInstance> OnItemSelected;

    private void Awake()
    {
        if (inventorySlots == null)
        {
            inventorySlots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>(includeInactive: true));
        }

        foreach(var slot in inventorySlots)
        {
            slot.OnItemSelected += () => OnItemSelected?.Invoke(slot.Item);
        }
    }

    public void SetInventory(List<ItemInstance> items)
    {
        if (inventorySlots == null)
        {
            inventorySlots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>(includeInactive: true));
        }

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
