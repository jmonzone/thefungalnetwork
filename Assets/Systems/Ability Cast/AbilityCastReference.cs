using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class AbilityCastReference : ScriptableObject
{
    //todo: remove shrune logic from ability cast
    [SerializeField] private ShruneItem shrune;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform target;
    [SerializeField] private float maxDistance;
    [SerializeField] private ShruneCollection shruneCollection;

    public Transform Target => target;
    public ShruneItem Shrune => shrune;
    public string ShruneId => shrune.name;
    public Vector3 StartPosition => origin.position + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public float MaxDistance => maxDistance;
    public Func<Attackable, bool> IsValidTarget { get; private set; }

    public event UnityAction OnShruneChanged;
    public event UnityAction OnStart;
    public event UnityAction OnUpdate;
    public event UnityAction OnCast;

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

    //todo: parameters should be optional and dependent on the ability type
    public void StartCast(Transform origin, Func<Attackable,bool> isValidTarget)
    {
        this.origin = origin;
        IsValidTarget = isValidTarget;
        OnStart?.Invoke();
    }

    //todo: parameters should be optional and dependent on the ability type
    public void StartCast(Transform origin, Transform target, Func<Attackable, bool> isValidTarget)
    {
        this.target = target;
        StartCast(origin, isValidTarget);
    }

    //todo: create aimed ability vs targeted ability
    public void UpdateCast()
    {
        OnUpdate?.Invoke();
    }

    public void UpdateCast(Vector3 direction)
    {
        Direction = direction;
        OnUpdate?.Invoke();
    }

    public void Cast()
    {
        OnCast?.Invoke();
    }

    public void Cast(Transform origin, Vector3 direction, Func<Attackable, bool> isValidTarget)
    {
        this.origin = origin;
        Direction = direction;
        IsValidTarget = isValidTarget;
        Cast();
    }
}
