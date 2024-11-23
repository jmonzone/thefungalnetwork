using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemButton;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    public ItemInstance Item { get; private set; }
    public event UnityAction OnItemSelected;

    private void Awake()
    {
        itemButton.onClick.AddListener(() => OnItemSelected?.Invoke());
    }

    public void SetItem(ItemInstance item)
    {
        Item = item;

        if (item)
        {
            itemImage.sprite = item.Data.Sprite;
            itemAmountText.text = item.Count.ToString();
        }

        itemImage.gameObject.SetActive(item);
        itemAmountText.gameObject.SetActive(item && item.Count > 1);
    }
}
