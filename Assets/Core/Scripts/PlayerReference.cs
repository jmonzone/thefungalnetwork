using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PlayerReference : ScriptableObject
{
    public Transform Transform { get; private set; }

    public event UnityAction OnPlayerUpdated;

    public void SetTransform(Transform transform)
    {
        Transform = transform;
        OnPlayerUpdated?.Invoke();
    }
}
