using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FungalSelectionItem : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private Image background;
    [SerializeField] private GameObject selectionOutline;

    public void SetData(FungalData data, UnityAction onClick)
    {
        image.sprite = data.ActionImage;
        //background.color = data.ActionColor;
        button.onClick.AddListener(() => onClick?.Invoke());
        gameObject.SetActive(true);
    }

    public void SetIsSelected(bool value)
    {
        selectionOutline.SetActive(value);
    }
}
