using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inventoryView;
    [SerializeField] private Button button;
    [SerializeField] private Image preview;

    public Button Button => button;

    private void Awake()
    {
        if (button) button.onClick.AddListener(() => navigation.Navigate(inventoryView));
    }

    private void OnEnable()
    {
        UpdatePreview();
        inventory.OnInventoryUpdated += UpdatePreview;
    }

    private void OnDisable()
    {
        inventory.OnInventoryUpdated -= UpdatePreview;

    }

    private void UpdatePreview()
    {
        var previewItem = inventory.LatestItem;

        if (previewItem)
        {
            preview.enabled = true;
            preview.sprite = previewItem.Data.Sprite;
        }
        else
        {
            preview.enabled = false;
        }
    }

}
