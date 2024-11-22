using UnityEngine;

public class GroveUI : MonoBehaviour
{
    [SerializeField] private bool debug;
    [SerializeField] private GameObject inputUI;
    [SerializeField] private InventoryService inventoryService;

    private void Awake()
    {
        inventoryService.OnInventoryOpened += () => ToggleUI(false);
        inventoryService.OnInventoryClosed += () => ToggleUI(true);

        if (!debug || !Application.isEditor)
        {
            ToggleUI(true);
        }
    }

    private void ToggleUI(bool input)
    {
        inputUI.SetActive(input);
    }
}
