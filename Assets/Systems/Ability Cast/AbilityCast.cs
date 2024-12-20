using System;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCast : MonoBehaviour
{
    [SerializeField] private ShruneItem data;
    public ShruneItem Data => data;

    public Vector3 StartPosition => transform.position + Direction.normalized;
    public Vector3 Direction { get; private set; }
    public bool IsValidTarget(Attackable attackable) => this.attackable != attackable;

    private Attackable attackable;

    public event UnityAction OnPrepare;
    public event UnityAction OnCastStart;
    public event UnityAction OnCastComplete;

    private void Awake()
    {
        attackable = GetComponent<Attackable>();
    }

    public void Prepare()
    {
        OnPrepare?.Invoke();
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }

    public void StartCast()
    {
        OnCastStart?.Invoke();
    }

    public void StartCast(Vector3 direction)
    {
        Direction = direction;
        StartCast();
    }

    public void CompleteCast()
    {
        OnCastComplete?.Invoke();
    }
}
