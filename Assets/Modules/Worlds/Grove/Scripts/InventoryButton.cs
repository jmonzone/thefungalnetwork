using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private ViewReference view;
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private Button button;
    [SerializeField] private Image preview;

    private void Awake()
    {
        if (button) button.onClick.AddListener(() => view.Open());

        inventory.OnItemAdded += item =>
        {
            preview.enabled = true;
            preview.sprite = item.Data.Sprite;
        };
    }

    private void Start()
    {
        if (inventory.Items.Count > 0)
        {
            preview.enabled = true;
            preview.sprite = inventory.Items.Last().Data.Sprite;
        }
        else
        {
            preview.enabled = false;
        }
    }
}
