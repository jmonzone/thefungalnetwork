using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image image;

    [SerializeField] private Item item;

    public void SetItem(Item item)
    {
        this.item = item;

        if (item)
        {
            nameText.text = item.Name;
            image.sprite = item.Sprite;

            if (descriptionText) descriptionText.text = item.Description;
        }

        nameText.gameObject.SetActive(item);
        if (descriptionText) descriptionText.gameObject.SetActive(item);
        image.enabled = item;
    }
}
