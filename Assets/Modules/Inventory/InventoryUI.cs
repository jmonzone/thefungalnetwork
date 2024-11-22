using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private ViewReference inventoryView;
    [SerializeField] private Navigation navigationService;

    private List<InventorySlot> inventorySlots;

    private void Awake()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>(includeInactive: true).ToList();

        inventoryView.OnOpened += () => UpdateInventorySlots();
    }

    private void UpdateInventorySlots()
    {
        int i = 0;
        foreach (var slot in inventorySlots)
        {
            if (i < inventory.Items.Count)
            {
                slot.SetItem(inventory.Items[i]);
            }
            else
            {
                slot.SetItem(null);
            }
            i++;
        }
    }
}
