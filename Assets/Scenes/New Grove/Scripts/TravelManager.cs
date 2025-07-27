using UnityEngine;
using UnityEngine.UI;

public class TravelManager : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Material chargedTreeMaterial;

    [SerializeField] private Light light1;
    [SerializeField] private Light light2;

    [SerializeField] private Renderer treeRenderer;

    private void Awake()
    {
        inventoryController.OnMushroomCountChanged += InventoryController_OnMushroomCountChanged;


        InventoryController_OnMushroomCountChanged(inventoryController.MushroomCount);

        unlockButton.onClick.AddListener(() =>
        {
            inventoryController.SetMushroomCount(inventoryController.MushroomCount - 5);
            light1.gameObject.SetActive(true);
            light2.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            treeRenderer.material = chargedTreeMaterial;
        });
    }

    private void InventoryController_OnMushroomCountChanged(int arg0)
    {
        unlockButton.interactable = inventoryController.MushroomCount >= 5f;
    }
}
