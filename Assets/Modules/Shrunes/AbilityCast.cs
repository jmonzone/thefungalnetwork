using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class AbilityCast : ScriptableObject
{
    [SerializeField] private ShruneItem shrune;
    [SerializeField] private Transform origin;

    public string ShruneId => shrune.name;
    public Vector3 StartPosition => origin.position + Vector3.up + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public float MaxDistance => shrune.MaxDistance;


    public event UnityAction OnStart;
    public event UnityAction OnUpdate;
    public event UnityAction OnComplete;

    public void StartCast(Transform origin, ShruneItem shrune)
    {
        this.origin = origin;
        this.shrune = shrune;
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
