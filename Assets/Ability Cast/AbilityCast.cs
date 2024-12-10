using UnityEngine;
using UnityEngine.Events;

// this scripts is used as a reference asset to handle ability casts
[CreateAssetMenu]
public class AbilityCast : ScriptableObject
{
    [SerializeField] private ShruneItem shrune;
    [SerializeField] private Transform origin;
    [SerializeField] private float maxDistance;

    public ShruneItem Shrune => shrune;
    public string ShruneId => shrune.name;
    public Vector3 StartPosition => origin.position + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public float MaxDistance => maxDistance;


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
        maxDistance = shrune.MaxDistance;
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

    public void CastImmediate(Transform origin, Vector3 direction)
    {
        this.origin = origin;
        Direction = direction;
        OnComplete?.Invoke();
    }
}
