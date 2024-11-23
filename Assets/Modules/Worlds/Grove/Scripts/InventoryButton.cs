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
    public ItemInstance PreviewItem { get; private set; }

    public event UnityAction OnDragStart;

    public bool ItemFilter(ItemInstance item) => item.Data.HasTags(itemTags);

    private void Awake()
    {
        if (button) button.onClick.AddListener(() => inventory.Open(ItemFilter));

        inventory.OnInventoryUpdated += () => UpdatePreview();
    }

    private void Start()
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        var filteredItems = inventory.GetFilteredItems(ItemFilter);

        if (filteredItems.Count > 0)
        {
            PreviewItem = filteredItems.Last();
            Debug.Log(filteredItems.Count);
            preview.enabled = true;
            preview.sprite = PreviewItem.Data.Sprite;
        }
        else
        {
            PreviewItem = null;
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
