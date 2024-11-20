using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    //todo: create image button component maybe like Inventory slot
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private Image inventoryPreview;

    [SerializeField] private GameObject mushroomButton;
    [SerializeField] private Image mushroomPreview;

    private Item targetItem;
    private GameObject spawnedItem;
    private Camera mainCamera;

    private ItemInstance MushroomItem => gameManager.Inventory.Find(item => item.Data.name == "Mushroom");
    private List<ItemInstance> MajorItems => gameManager.Inventory.FindAll(item => item != MushroomItem).ToList();

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (gameManager.Inventory.Count > 0)
        {
            if (MushroomItem)
            {
                mushroomPreview.enabled = true;
                mushroomPreview.sprite = MushroomItem.Data.Sprite;
            }

            if (MajorItems.Count > 0)
            {
                targetItem = MajorItems[0].Data;
                inventoryPreview.enabled = true;
                inventoryPreview.sprite = targetItem.Sprite;
            }
        }
    }

    public void SpawnMushroom()
    {
        if (MushroomItem) SpawnItem(MushroomItem);
    }

    public void SpawnMajorItem()
    {
        if (MajorItems.Count > 0) SpawnItem(MajorItems[0]);
    }

    private void SpawnItem(ItemInstance item)
    {
        spawnedItem = Instantiate(item.Data.ItemPrefab);
        inventoryButton.SetActive(false);
        mushroomButton.SetActive(false);
    }

    public void DropItem()
    {
        spawnedItem = null;
        inventoryButton.SetActive(true);
        mushroomButton.SetActive(true);
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
