using System;
using UnityEngine;
using UnityEngine.Events;

//todo: remove ability cast from shurne logic, maybe
// this scripts is used as a reference asset to handle ability casts
[CreateAssetMenu]
public class AbilityCast : ScriptableObject
{
    [SerializeField] private ShruneItem shrune;
    [SerializeField] private Transform origin;
    [SerializeField] private float maxDistance;
    [SerializeField] private ShruneCollection shruneCollection;

    public ShruneItem Shrune => shrune;
    public string ShruneId => shrune.name;
    public Vector3 StartPosition => origin.position + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public float MaxDistance => maxDistance;
    public Func<Attackable, bool> IsValidTarget { get; private set; }

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

    //todo: parameters should be optional and dependent on the ability type
    public void StartCast(Transform origin, Func<Attackable,bool> isValidTarget)
    {
        this.origin = origin;
        IsValidTarget = isValidTarget;
        OnStart?.Invoke();
    }

    public void UpdateCast(Vector3 direction)
    {
        Direction = direction;
        OnUpdate?.Invoke();
    }

    public void Cast()
    {
        if (this.shrune && shruneCollection.TryGetShruneById(ShruneId, out ShruneItem shrune))
        {
            var projectile = Instantiate(shrune.ProjectilePrefab, StartPosition + Vector3.up * 0.5f, Quaternion.LookRotation(Direction));
            projectile.Shoot(Direction, shrune.MaxDistance, shrune.Speed, IsValidTarget);
        }

        OnComplete?.Invoke();
    }

    public void Cast(Transform origin, Vector3 direction, Func<Attackable, bool> isValidTarget)
    {
        this.origin = origin;
        Direction = direction;
        IsValidTarget = isValidTarget;
        Cast();
    }
}
