using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemButton;

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
        }

        itemImage.gameObject.SetActive(item);
    }
}
