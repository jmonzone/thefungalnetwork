using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryViewReference inventory;

    private List<InventorySlot> inventorySlots;

    private void Awake()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>(includeInactive: true).ToList();

        inventory.OnOpened += () => UpdateInventorySlots();
    }

    private void UpdateInventorySlots()
    {
        var items = inventory.GetFilteredItems();

        int i = 0;

        foreach (var slot in inventorySlots)
        {
            if (i < items.Count)
            {
                slot.SetItem(items[i]);
            }
            else
            {
                slot.SetItem(null);
            }
            i++;
        }
    }
}
