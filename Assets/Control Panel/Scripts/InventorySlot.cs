using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;

    public void SetItem(Item item)
    {
        itemImage.sprite = item.Sprite;
        itemImage.gameObject.SetActive(true);
    }
}
