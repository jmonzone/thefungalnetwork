using TMPro;
using UnityEngine;

public class SporeCountUI : MonoBehaviour
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private TextMeshProUGUI mushroomCountText;

    private void Start()
    {
        mushroomCountText.text = inventory.SporeCount.ToString();
    }

    private void OnEnable()
    {
        mushroomCountText.text = inventory.SporeCount.ToString();
        inventory.OnSporeCountChanged += Inventory_OnSporeCountChanged;
    }

    private void OnDisable()
    {
        inventory.OnSporeCountChanged -= Inventory_OnSporeCountChanged;
    }

    private void Inventory_OnSporeCountChanged(int arg0)
    {
        mushroomCountText.text = inventory.SporeCount.ToString();
    }
}
