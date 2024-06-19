using UnityEngine;
using UnityEngine.Events;

public class ProximityAction : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color color;
    [SerializeField] private string text;

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public Color Color { get => color; set => color = value; }
    public string Text { get => text; set => text = value; }

    public event UnityAction OnUse;

    public void Use()
    {
        OnUse?.Invoke();
    }
}
