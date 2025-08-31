using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Item item;

    public Item Item => item;

    public event UnityAction OnClick;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClick?.Invoke());
    }

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
