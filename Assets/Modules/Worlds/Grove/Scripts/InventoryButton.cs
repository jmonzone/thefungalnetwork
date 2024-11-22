using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private ViewReference view;
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private Button button;
    [SerializeField] private Image preview;

    public Button Button => button;
    public ItemInstance PreviewItem { get; private set; }

    private Predicate<ItemInstance> filter;

    public event UnityAction OnDragStart;

    private void Awake()
    {
        if (button) button.onClick.AddListener(() => view.Open());

        inventory.OnInventoryUpdated += () => UpdatePreview();
    }

    private void Start()
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        var items = filter != null ? inventory.Items.FindAll(filter) : inventory.Items;

        Debug.Log($"filter {name} {items.Count}");

        if (items.Count > 0)
        {
            PreviewItem = items.Last();
            preview.enabled = true;
            preview.sprite = PreviewItem.Data.Sprite;
        }
        else
        {
            PreviewItem = null;
            preview.enabled = false;
        }
    }

    public void ApplyFilter(Predicate<ItemInstance> filter)
    {
        this.filter = filter;
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
