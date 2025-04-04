using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private ItemTags itemTags;
    [SerializeField] private Button exitButton;
    [SerializeField] private MultiplayerReference multiplayerManager;

    private List<InventorySlot> inventorySlots;

    public bool ItemFilter(ItemInstance item) => item.Data.HasOneOfTag(itemTags);
    public event UnityAction<InventorySlot> OnItemDragged;

    private void Awake()
    {
        exitButton.onClick.AddListener(() =>
        {
            multiplayerManager.RequestDisconnect();
        });

        inventorySlots = GetComponentsInChildren<InventorySlot>(includeInactive: true).ToList();
        foreach(var slot in inventorySlots)
        {
            slot.OnItemDragged += () => OnItemDragged?.Invoke(slot);
        }
    }

    private void OnEnable()
    {
        UpdateInventorySlots();
        inventory.OnInventoryUpdated += UpdateInventorySlots;

    }

    private void OnDisable()
    {
        inventory.OnInventoryUpdated -= UpdateInventorySlots;
    }

    private void UpdateInventorySlots()
    {
        var items = inventory.GetFilteredItems(ItemFilter);


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
