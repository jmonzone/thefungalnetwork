using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image actionImage;
    [SerializeField] private Image background;
    [SerializeField] private SlideAnimation slideAnimation;
    [SerializeField] private TextMeshProUGUI text;

    public EntityController Entity { get; set; }

    public event UnityAction<EntityController> OnClicked;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClicked?.Invoke(Entity));
    }

    public void SetEntity(EntityController entity)
    {
        Entity = entity;
        SetVisible(Entity);

        if (Entity)
        {
            actionImage.sprite = entity.ActionImage;
            background.color = entity.ActionColor;
            text.text = entity.ActionText;
        }
    }

    public void SetVisible(bool value)
    {
        button.interactable = value;
        slideAnimation.IsVisible = value;
    }

}
