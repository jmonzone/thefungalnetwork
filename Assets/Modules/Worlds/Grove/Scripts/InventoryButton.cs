using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryViewReference inventory;
    [SerializeField] private Button button;
    [SerializeField] private Image preview;
    [SerializeField] private ItemTags itemTags;

    public Button Button => button;
    public ItemInstance PreviewItem => inventory.GetFilteredItems(ItemFilter).LastOrDefault();

    public bool ItemFilter(ItemInstance item) => item.Data.HasTags(itemTags);

    private void Awake()
    {
        if (button) button.onClick.AddListener(() => inventory.Open(ItemFilter));
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
