using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button closeButton;

    private List<InventorySlot> inventorySlots;

    public event UnityAction OnCloseButtonClicked;

    private void Awake()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>(includeInactive: true).ToList();
        closeButton.onClick.AddListener(() => OnCloseButtonClicked?.Invoke());
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
            if (i < gameManager.Inventory.Count)
            {
                slot.SetItem(gameManager.Inventory[i]);
            }
            else
            {
                slot.SetItem(null); // Clear the slot if no item exists
            }
            i++;
        }
    }
}
