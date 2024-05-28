using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image actionImage;
    [SerializeField] private Image background;
    [SerializeField] private Transform hiddenAnchor;
    [SerializeField] private Transform visibleAnchor;
    [SerializeField] private TextMeshProUGUI text;

    private bool isVisible;
    private EntityController entity;

    public event UnityAction<EntityController> OnClicked;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClicked?.Invoke(entity));
        transform.position = hiddenAnchor.transform.position;
    }

    public void SetInteraction(EntityController entity)
    {
        this.entity = entity;
        actionImage.sprite = entity.ActionImage;
        background.color = entity.Color;
        text.text = entity switch {
            FungalController _ => "Talk",
            EggController _ => "Hatch",
            _ => "???"
        };
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
        button.interactable = value;
    }

    private void Update()
    {
        var targetPosition = isVisible ? visibleAnchor.position : hiddenAnchor.position;
        var direction = targetPosition - transform.position;
        if (direction.magnitude > 10f)
        {
            direction.y = 0;
            transform.position += 5f * Time.deltaTime * direction + direction.normalized;
        }
        else transform.position = targetPosition;
    }

}
