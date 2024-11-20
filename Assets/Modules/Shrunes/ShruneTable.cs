using UnityEngine;
using UnityEngine.UI;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private Image inventoryPreview;

    private Item targetItem;
    private GameObject item;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (gameManager.Inventory.Count > 0)
        {
            targetItem = gameManager.Inventory[0].Data;
            inventoryPreview.enabled = true;
            inventoryPreview.sprite = targetItem.Sprite;
        }
    }

    public void SpawnItem()
    {
        item = Instantiate(targetItem.ItemPrefab);
        inventoryButton.SetActive(false);
    }

    public void DropItem()
    {
        item = null;
        inventoryButton.SetActive(true);
    }

    private void Update()
    {
        if (item)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                item.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                DropItem();
            }
        }
    }
}
