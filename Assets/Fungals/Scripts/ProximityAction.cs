using UnityEngine;
using UnityEngine.Events;

public class ProximityAction : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color color;
    [SerializeField] private string text;
    [SerializeField] private UnityEvent onUse;

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public Color Color { get => color; set => color = value; }
    public string Text { get => text; set => text = value; }
    public bool Interactable { get; set; }

    public event UnityAction OnUse
    {
        add => onUse.AddListener(value);
        remove => onUse.RemoveListener(value);
    }

    public void Use()
    {
        onUse?.Invoke();
    }

    public void SetInteractable(bool value)
    {
        Interactable = value;
    }
}
