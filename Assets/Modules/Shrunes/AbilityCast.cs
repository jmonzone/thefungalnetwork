using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class AbilityCast : ScriptableObject
{
    private Transform origin;

    public Vector3 StartPosition => origin.position;
    public Vector3 Direction { get; private set; }

    public float MaxDistance { get; private set; }

    public event UnityAction OnStart;
    public event UnityAction OnUpdate;
    public event UnityAction OnComplete;

    public void StartCast(Transform origin, float maxDistance)
    {
        this.origin = origin;
        MaxDistance = maxDistance;
        OnStart?.Invoke();
    }

    public void UpdateCast(Vector3 direction)
    {
        Direction = direction;
        OnUpdate?.Invoke();
    }

    public void EndCast()
    {
        OnComplete?.Invoke();
    }
}
