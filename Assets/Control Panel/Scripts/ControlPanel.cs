using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject inventory;

    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Transform inventorySlotAnchor;

    private List<InventorySlot> inventorySlots;

    private enum State
    {
        JOYSTICK,
        INVENTORY
    }

    private void Awake()
    {
        inventoryButton.onClick.AddListener(() => SetState(State.INVENTORY));
        closeButton.onClick.AddListener(() => SetState(State.JOYSTICK));

        inventorySlots = new List<InventorySlot>(inventorySlotAnchor.GetComponentsInChildren<InventorySlot>(includeInactive: true));
        SetState(State.JOYSTICK);
    }

    private void SetState(State state)
    {
        joystick.SetActive(state == State.JOYSTICK);
        inventory.SetActive(state == State.INVENTORY);
    }

    public void SetInventory(List<Item> items)
    {
        var i = 0;
        foreach(var item in items)
        {
            inventorySlots[i].SetItem(item);
            i++;
        }
    }
}
