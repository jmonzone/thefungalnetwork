using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image actionImage;
    [SerializeField] private Image background;
    [SerializeField] private SlideAnimation slideAnimation;
    [SerializeField] private TextMeshProUGUI text;

    public Vector3 TargetPosition { get; set; }
    public int Order { get; set; } = -1;
    public EntityController Entity { get; set; }

    private void Awake()
    {

        button.onClick.AddListener(() => Entity.UseAction());
    }

    private void Update()
    {
        if (Entity)
        {
            var direction = TargetPosition - transform.parent.position;

            //Debug.Log($"{transform.parent.name} {targetPosition} {direction.magnitude}");

            if (direction.magnitude > 10f)
            {
                transform.parent.position += 5f * Time.deltaTime * direction + direction.normalized;
            }
            else transform.parent.position = TargetPosition;
        }
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
        else
        {
            Order = 99;
        }
    }

    public void SetVisible(bool value)
    {
        button.interactable = value;
        slideAnimation.IsVisible = value;
    }

}
