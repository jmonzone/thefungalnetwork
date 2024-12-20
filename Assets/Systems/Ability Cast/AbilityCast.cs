using System;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCast : MonoBehaviour
{
    //todo: remove shrune logic from ability cast
    [SerializeField] private ShruneItem shrune;
    [SerializeField] private float maxDistance;

    public ShruneItem Shrune => shrune;
    public string ShruneId => shrune.name;
    public Vector3 StartPosition => transform.position + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public float MaxDistance => maxDistance;
    public bool IsValidTarget(Attackable attackable) => this.attackable != attackable;

    private Attackable attackable;

    public event UnityAction OnShruneChanged;
    public event UnityAction OnStart;
    public event UnityAction OnUpdate;
    public event UnityAction OnCast;

    private void Awake()
    {
        attackable = GetComponent<Attackable>();
        if (shrune) maxDistance = shrune.MaxDistance;
    }

    //todo: remove
    public void SetShrune(ShruneItem shrune)
    {
        this.shrune = shrune;
        maxDistance = shrune.MaxDistance;
        OnShruneChanged?.Invoke();
    }

    public void StartCast()
    {
        OnStart?.Invoke();
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }

    public void Cast()
    {
        OnCast?.Invoke();
    }

    public void Cast(Vector3 direction)
    {
        Direction = direction;
        Cast();
    }
}
