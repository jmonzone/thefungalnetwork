using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellbookPageUI : MonoBehaviour
{
    [SerializeField] private SpellbookReference spellbook;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private BuyButton castButton;
    [SerializeField] private bool isUnlocked = false;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image image;

    private Item item;

    private void Awake()
    {
        castButton.OnBuy += () =>
        {
            if (item)
            {
                inventory.SummonItem(item);
                spellbook.Close();
            }
        };
    }

    public void SetItem(Item item)
    {
        this.item = item;

        if (item)
        {
            castButton.SetPrice(item.Price);
            nameText.text = item.Name;
            descriptionText.text = item.Description;
            image.sprite = item.Sprite;
        }
        else
        {
            nameText.text = "???";
        }

        descriptionText.gameObject.SetActive(item);
        image.enabled = item;
        castButton.gameObject.SetActive(item);
    }
}
