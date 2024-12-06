using UnityEngine;
using UnityEngine.Events;

// this scripts is used as a reference asset to handle ability casts
[CreateAssetMenu]
public class AbilityCast : ScriptableObject
{
    [SerializeField] private ShruneItem shrune;
    [SerializeField] private Transform origin;

    public ShruneItem Shrune => shrune;
    public string ShruneId => shrune.name;
    public Vector3 StartPosition => origin.position + Vector3.up + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public float MaxDistance => shrune.MaxDistance;


    public event UnityAction OnShruneChanged;
    public event UnityAction OnStart;
    public event UnityAction OnUpdate;
    public event UnityAction OnComplete;

    public void Reset()
    {
        shrune = null;
    }

    public void SetShrune(ShruneItem shrune)
    {
        this.shrune = shrune;
        OnShruneChanged?.Invoke();
    }

    public void StartCast(Transform origin)
    {
        this.origin = origin;
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
