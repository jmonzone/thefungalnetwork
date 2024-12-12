using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemButton;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    public ItemInstance Item { get; private set; }
    public event UnityAction OnItemSelected;
    public event UnityAction OnItemDragged;

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

        itemImage.enabled = item;
        itemImage.gameObject.SetActive(item);
        itemAmountText.gameObject.SetActive(item && item.Count > 1);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        OnItemDragged?.Invoke();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }
}
