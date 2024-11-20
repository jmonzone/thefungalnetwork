using UnityEngine;
using UnityEngine.Events;

public class ProximityAction : MonoBehaviour
{
    [SerializeField] private bool interactable = true;
    public bool Interactable => interactable;

    public bool InRange { get; set; }

    public event UnityAction OnUse;
    public event UnityAction<bool> OnInRangeChanged;

    public void Use()
    {
        OnUse?.Invoke();
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
    }

    public void SetInRange(bool value)
    {
        InRange = value;
        OnInRangeChanged?.Invoke(value);
    }
}
