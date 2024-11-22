using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryService inventoryService;
    [SerializeField] private GameObject view;
    [SerializeField] private Button closeButton;

    private List<InventorySlot> inventorySlots;


    private void Awake()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>(includeInactive: true).ToList();
        closeButton.onClick.AddListener(() => inventoryService.CloseInventory());

        inventoryService.OnInventoryOpened += () => view.SetActive(true);
        inventoryService.OnInventoryClosed += () => view.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateInventorySlots();
    }

    private void Start()
    {
        UpdateInventorySlots();
    }

    private void UpdateInventorySlots()
    {
        int i = 0;
        foreach (var slot in inventorySlots)
        {
            if (i < inventoryService.Inventory.Count)
            {
                slot.SetItem(inventoryService.Inventory[i]);
            }
            else
            {
                slot.SetItem(null); // Clear the slot if no item exists
            }
            i++;
        }
    }
}
