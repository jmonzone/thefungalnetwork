using UnityEngine;
using UnityEngine.Events;

public class ProximityAction : MonoBehaviour
{
    public bool Interactable { get; set; }

    public event UnityAction OnUse;

    public void Use()
    {
        OnUse?.Invoke();
    }

    public void SetInteractable(bool value)
    {
        Interactable = value;
    }
}
