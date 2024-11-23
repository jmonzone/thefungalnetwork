using System.Collections.Generic;
using UnityEngine;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private ItemInventory inventory;
    [SerializeField] private InventoryButton majorItemButton;
    [SerializeField] private InventoryButton minorItemButton;

    [SerializeField] private Item shruneItem;
    [SerializeField] private Transform shruneSpawnPosition;
    [SerializeField] private ViewReference viewReference;

    private Camera mainCamera;

    private GameObject selectedItem;
    private GameObject majorItemObject;
    private List<GameObject> minorItemObjects = new List<GameObject>();

    private GameObject shrune;
    private bool shrunePickedUp;

    private void Awake()
    {
        mainCamera = Camera.main;
        minorItemButton.OnDragStart += () => SpawnMinorItem();
        majorItemButton.OnDragStart += () => SpawnMajorItem();

        viewReference.OnOpened += UpdateView;
    }

    public void SpawnMinorItem()
    {
        var item = SpawnItem(minorItemButton.PreviewItem);
        minorItemObjects.Add(item);
    }

    public void SpawnMajorItem()
    {
        majorItemObject = SpawnItem(majorItemButton.PreviewItem);
    }

    private GameObject SpawnItem(ItemInstance item)
    {
        inventory.RemoveFromInventory(item.Data, 1);
        selectedItem = Instantiate(item.Data.ItemPrefab);
        UpdateView();
        return selectedItem;
    }

    private void DropItem()
    {
        selectedItem = null;

        if (majorItemObject && minorItemObjects.Count == 5)
        {
            SpawnShrune();

            
        }

        UpdateView();
    }

    private void SpawnShrune()
    {
        shrune = Instantiate(shruneItem.ItemPrefab, shruneSpawnPosition.position, Quaternion.identity);

        majorItemObject.SetActive(false);
        majorItemObject = null;

        foreach (var item in minorItemObjects)
        {
            item.SetActive(false);
        }

        minorItemObjects = new List<GameObject>();

    }

    private void UpdateView()
    {
        majorItemButton.Button.interactable = !selectedItem && !majorItemObject && majorItemButton.PreviewItem;
        minorItemButton.Button.interactable = !selectedItem && minorItemObjects.Count < 5 && minorItemButton.PreviewItem;
    }

    private void Update()
    {
        if (selectedItem)
        {
            if (TryRaycast(out RaycastHit hit))
            {
                selectedItem.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                DropItem();
            }
        }
        else if (shrune && !shrunePickedUp)
        {
            if (TryRaycast(out RaycastHit hit))
            {
                if (hit.transform.GetComponentInParent<Transform>() == shrune.transform)
                {
                    shrunePickedUp = true;
                    Invoke(nameof(HideShrune), 2f);
                }
            }
        }
    }

    private void HideShrune()
    {
        inventory.AddToInventory(shruneItem, 1);
        shrune.SetActive(false);
        shrune = null;
        shrunePickedUp = false;
    }

    private bool TryRaycast(out RaycastHit hit)
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, 10f);
    }
}
