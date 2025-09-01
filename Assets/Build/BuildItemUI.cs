using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryReference inventory;

    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image sporeImage;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Settings")]
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    [Header("Runtime")]
    [SerializeField] private Item item;

    public Item Item => item;

    public event UnityAction OnClick;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClick?.Invoke());
    }

    private void OnEnable()
    {
        Inventory_OnSporeCountChanged(inventory.SporeCount);
        inventory.OnSporeCountChanged += Inventory_OnSporeCountChanged;
    }

    private void OnDisable()
    {
        inventory.OnSporeCountChanged -= Inventory_OnSporeCountChanged;
    }

    private void Inventory_OnSporeCountChanged(int spore)
    {
        if (item)
        {
            var canPurchase = spore >= item.Price;
            button.interactable = canPurchase;
            sporeImage.color = canPurchase ? activeColor : inactiveColor;
            priceText.color = canPurchase ? activeColor : inactiveColor;
        }
    }

    public void SetItem(Item item)
    {
        this.item = item;

        if (item)
        {
            nameText.text = item.Name;
            image.sprite = item.Sprite;
            priceText.text = item.Price.ToString();
        }

        nameText.gameObject.SetActive(item);
        priceText.gameObject.SetActive(item);
        image.enabled = item;
    }
}
