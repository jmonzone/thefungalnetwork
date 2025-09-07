using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryReference inventory;

    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Runtime")]
    [SerializeField] private Item item;

    public Item Item => item;

    private void Awake()
    {
        button.onClick.AddListener(() => inventory.SelectItem(item));
    }

    public void SetItem(Item item)
    {
        this.item = item;

        if (item)
        {
            nameText.text = item.Name;
            image.sprite = item.Sprite;
        }

        nameText.gameObject.SetActive(item);
        image.enabled = item;
    }
}
