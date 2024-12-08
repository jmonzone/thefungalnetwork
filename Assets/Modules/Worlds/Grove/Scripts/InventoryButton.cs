using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private ViewReference inventoryView;
    [SerializeField] private Button button;
    [SerializeField] private Image preview;

    public Button Button => button;
    public ItemInstance PreviewItem => inventory.Items.LastOrDefault();

    private void Awake()
    {
        if (button) button.onClick.AddListener(() => inventoryView.RequestShow());
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
        if (PreviewItem)
        {
            preview.enabled = true;
            preview.sprite = PreviewItem.Data.Sprite;
        }
        else
        {
            preview.enabled = false;
        }
    }

}
