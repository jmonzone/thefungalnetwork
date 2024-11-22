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
}
