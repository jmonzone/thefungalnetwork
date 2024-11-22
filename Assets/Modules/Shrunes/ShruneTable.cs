using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private ItemInventory inventoryService;
    [SerializeField] private InventoryButton majorItemButton;
    [SerializeField] private InventoryButton minorItemButton;

    [SerializeField] private GameObject shrunePrefab;
    [SerializeField] private Transform shruneSpawnPosition;

    private Camera mainCamera;

    private GameObject spawnedItem;
    private GameObject majorItem;
    private List<GameObject> minorItems = new List<GameObject>();
    private GameObject shrune;

    private void Awake()
    {
        mainCamera = Camera.main;
        minorItemButton.OnDragStart += () => SpawnMinorItem();
        majorItemButton.OnDragStart += () => SpawnMajorItem();
        majorItemButton.ApplyFilter(item => item.Data.name != "Mushroom");
        minorItemButton.ApplyFilter(item => item.Data.name == "Mushroom");
    }

    public void SpawnMinorItem()
    {
        var mushroom = SpawnItem(minorItemButton.PreviewItem);
        minorItems.Add(mushroom);
    }

    public void SpawnMajorItem()
    {
        majorItem = SpawnItem(majorItemButton.PreviewItem);
    }

    private GameObject SpawnItem(ItemInstance item)
    {
        spawnedItem = Instantiate(item.Data.ItemPrefab);
        UpdateView();
        return spawnedItem;
    }

    private void DropItem()
    {
        spawnedItem = null;
        UpdateView();

        if (majorItem && minorItems.Count == 5)
        {
            shrune = Instantiate(shrunePrefab, shruneSpawnPosition.position, Quaternion.identity);

            majorItem.SetActive(false);
            majorItem = null;

            foreach(var mushroom in minorItems)
            {
                mushroom.SetActive(false);
            }

            minorItems = new List<GameObject>();
            UpdateView();
        }
    }

    private void UpdateView()
    {
        majorItemButton.Button.interactable = !spawnedItem && !majorItem;
        minorItemButton.Button.interactable = !spawnedItem && minorItems.Count < 5;
    }

    private void Update()
    {
        if (spawnedItem)
        {
            if (TryRaycast(out RaycastHit hit))
            {
                spawnedItem.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                DropItem();
            }
        }
        else if (shrune)
        {
            if (TryRaycast(out RaycastHit hit))
            {
                if (hit.transform.GetComponentInParent<Transform>() == shrune.transform)
                {
                    shrune.SetActive(false);
                    shrune = null;
                }
            }
        }
    }

    private bool TryRaycast(out RaycastHit hit)
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, 10f);
    }
}
