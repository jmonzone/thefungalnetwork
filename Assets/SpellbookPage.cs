using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellbookPageUI : MonoBehaviour
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Button summonButton;
    [SerializeField] private TextMeshProUGUI notEnoughSporesText;
    [SerializeField] private bool isUnlocked = false;

    private void OnEnable()
    {
        Inventory_OnSporeCountChanged(inventory.SporeCount);
        inventory.OnSporeCountChanged += Inventory_OnSporeCountChanged;
    }

    private void OnDisable()
    {
        inventory.OnSporeCountChanged -= Inventory_OnSporeCountChanged;
    }

    private void Inventory_OnSporeCountChanged(int value)
    {
        if (isUnlocked)
        {
            summonButton.interactable = value >= 125;
            notEnoughSporesText.gameObject.SetActive(value < 125);
        }
        else
        {
            summonButton.interactable = false;
            notEnoughSporesText.gameObject.SetActive(false);
        }
    }
}
