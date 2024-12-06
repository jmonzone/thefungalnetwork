using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private InventoryViewReference inventory;
    [SerializeField] private Button button;
    [SerializeField] private Image preview;
    [SerializeField] private ItemTags itemTags;

    public Button Button => button;
    public ItemInstance PreviewItem => inventory.GetFilteredItems(ItemFilter).LastOrDefault();

    public event UnityAction OnDragStart;

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

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (button.interactable)
        {
            OnDragStart?.Invoke();
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }
}
