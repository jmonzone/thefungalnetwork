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
    private UnitWander unitWander;
    private UnitDialogue unitDialogue;
    private UnitFollow unitFollow;

    protected override void Awake()
    {
        base.Awake();

        unitWander = GetComponent<UnitWander>();
        unitDialogue = GetComponent<UnitDialogue>();
        unitFollow = GetComponent<UnitFollow>();
    }

    public void SetDestination(Vector3 destination, Vector3 direction)
    {
        transform.position = destination;
        transform.forward = direction;
    }
}
