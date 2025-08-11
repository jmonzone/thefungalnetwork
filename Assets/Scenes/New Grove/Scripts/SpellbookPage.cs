using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellbookPageUI : MonoBehaviour
{
    [SerializeField] private SpellbookReference spellbook;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private BuyButton castButton;
    [SerializeField] private bool isUnlocked = false;

    private void Awake()
    {
        castButton.OnBuy += () =>
        {
            inventory.SummonItem(125);
            spellbook.Close();
        };

        if (isUnlocked) castButton.SetPrice(125);
        else castButton.gameObject.SetActive(false);
    }
}
