using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryService inventoryService;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Image inventoryPreview;

    private void Awake()
    {
        if (inventoryButton) inventoryButton.onClick.AddListener(() => inventoryService.OpenInventory());

        inventoryService.OnItemAdded += item =>
        {
            inventoryPreview.enabled = true;
            inventoryPreview.sprite = item.Data.Sprite;
        };
    }

    private void OnEnable()
    {
        if (inventoryService.Inventory.Count > 0)
        {
            inventoryPreview.enabled = true;
            inventoryPreview.sprite = inventoryService.Inventory.Last().Data.Sprite;
        }
        else
        {
            inventoryPreview.enabled = false;
        }
    }
}
