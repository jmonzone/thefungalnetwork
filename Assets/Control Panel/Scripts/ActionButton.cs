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

    private bool isVisible;

    public event UnityAction OnClicked;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClicked?.Invoke());
        transform.position = hiddenAnchor.transform.position;
    }

    public void SetInteraction(Sprite sprite, Color color)
    {
        actionImage.sprite = sprite;
        background.color = color;
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
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
