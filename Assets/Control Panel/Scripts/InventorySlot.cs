using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;

    public void SetItem(Item item)
    {
        if (item)
        {
            itemImage.sprite = item.Sprite;
        }

        itemImage.gameObject.SetActive(item);
    }
}
