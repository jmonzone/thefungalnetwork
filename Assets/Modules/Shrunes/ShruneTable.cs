using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private ItemInventory inventory;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Button resetButton;

    [SerializeField] private Item shruneItem;
    [SerializeField] private Transform shruneSpawnPosition;

    private Camera mainCamera;

    private GameObject selectedItem;
    private Dictionary<GameObject, ItemInstance> itemObjectsMap = new Dictionary<GameObject, ItemInstance>();

    private GameObject shrune;
    private bool shrunePickedUp;

    private void Awake()
    {
        mainCamera = Camera.main;

        inventoryUI.OnItemDragged += slot => SpawnItem(slot.Item);
        resetButton.onClick.AddListener(() =>
        {
            foreach (var obj in itemObjectsMap.Keys)
            {
                inventory.AddToInventory(itemObjectsMap[obj].Data, 1);
            }

            ResetItems();
        });
    }

    private void SpawnItem(ItemInstance item)
    {
        inventory.RemoveFromInventory(item.Data, 1);
        selectedItem = Instantiate(item.Data.ItemPrefab);
        itemObjectsMap.Add(selectedItem, item);
    }

    private void DropItem()
    {
        selectedItem = null;

        if (itemObjectsMap.Count == 6)
        {
            SpawnShrune();
        }
    }

    private void SpawnShrune()
    {
        shrune = Instantiate(shruneItem.ItemPrefab, shruneSpawnPosition.position, Quaternion.identity);

        ResetItems();

    }

    private void ResetItems()
    {
        foreach(var obj in itemObjectsMap.Keys)
        {
            obj.SetActive(false);
        }

        itemObjectsMap = new Dictionary<GameObject, ItemInstance>();
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
