using UnityEngine;
using UnityEngine.Events;

public interface IJob
{
    public bool IsAble { get; }
    public bool IsMoving { get; }
    public Vector3 TargetPosition { get; }
    public event UnityAction OnIsAbleChanged;
    public event UnityAction OnIsMovingChanged;
}

public enum FungalState
{
    WANDER,
    DIALOGUE,
    FOLLOW
}

public class FungalController : UnitController
{
    public bool IsAtDestination => unitDestination.IsAtDestination;

    private UnitDestination unitDestination;
    private UnitFollow unitFollow;

    protected override void Awake()
    {
        base.Awake();

        unitDestination = GetComponent<UnitDestination>();
        unitFollow = GetComponent<UnitFollow>();
    }

    public void SetDestination(Vector3 destination)
    {
        unitDestination.SetDestination(destination);
        SetBehaviour(unitDestination);
    }

    public void SetTarget(Transform target)
    {
        unitFollow.SetTarget(target);
        SetBehaviour(unitFollow);
    }
}
