using System.Collections.Generic;
using UnityEngine;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private ItemInventory inventory;
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField] private Item shruneItem;
    [SerializeField] private Transform shruneSpawnPosition;

    private Camera mainCamera;

    private GameObject selectedItem;
    private List<GameObject> spawnedItems = new List<GameObject>();

    private GameObject shrune;
    private bool shrunePickedUp;

    private void Awake()
    {
        mainCamera = Camera.main;

        inventoryUI.OnItemDragged += slot => SpawnItem(slot.Item);
    }

    private void SpawnItem(ItemInstance item)
    {
        inventory.RemoveFromInventory(item.Data, 1);
        selectedItem = Instantiate(item.Data.ItemPrefab);
        spawnedItems.Add(selectedItem);
    }

    private void DropItem()
    {
        selectedItem = null;

        if (spawnedItems.Count == 6)
        {
            SpawnShrune();
        }
    }

    private void SpawnShrune()
    {
        shrune = Instantiate(shruneItem.ItemPrefab, shruneSpawnPosition.position, Quaternion.identity);

        foreach (var item in spawnedItems)
        {
            item.SetActive(false);
        }

        spawnedItems = new List<GameObject>();

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
