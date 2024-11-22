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

    private Item targetItem;
    private GameObject spawnedItem;
    private Camera mainCamera;

    private GameObject majorItem;
    private List<GameObject> mushrooms = new List<GameObject>();

    private ItemInstance MushroomItem => inventoryService.Items.Find(item => item.Data.name == "Mushroom");
    private List<ItemInstance> MajorItems => inventoryService.Items.FindAll(item => item != MushroomItem).ToList();

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void SpawnMushroom()
    {
        if (MushroomItem)
        {
            var mushroom = SpawnItem(MushroomItem);
            mushrooms.Add(mushroom);
        }
    }

    public void SpawnMajorItem()
    {
        if (MajorItems.Count > 0)
        {
            majorItem = SpawnItem(MajorItems[0]);
        }
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

        if (majorItem && mushrooms.Count == 5)
        {
            Instantiate(shrunePrefab, shruneSpawnPosition.position, Quaternion.identity);

            majorItem.SetActive(false);
            majorItem = null;

            foreach(var mushroom in mushrooms)
            {
                mushroom.SetActive(false);
            }

            mushrooms = new List<GameObject>();
            UpdateView();
        }
    }

    private void UpdateView()
    {
        majorItemButton.gameObject.SetActive(!spawnedItem && !majorItem);
        minorItemButton.gameObject.SetActive(!spawnedItem && mushrooms.Count < 5);
    }

    private void Update()
    {
        if (spawnedItem)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                spawnedItem.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                DropItem();
            }
        }
    }
}
