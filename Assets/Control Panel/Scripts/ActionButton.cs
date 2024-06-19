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
    public ProximityAction ProximityAction { get; set; }

    private void Awake()
    {
        button.onClick.AddListener(() => ProximityAction.Use());
    }

    private void Update()
    {
        if (ProximityAction)
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

    public void SetProximityAction(ProximityAction proximityAction)
    {
        ProximityAction = proximityAction;
        SetVisible(ProximityAction);

        if (ProximityAction)
        {
            actionImage.sprite = proximityAction.Sprite;
            background.color = proximityAction.Color;
            text.text = proximityAction.Text;
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
