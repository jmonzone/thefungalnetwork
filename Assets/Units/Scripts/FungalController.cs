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

public class FungalController : UnitController, IInteractable
{
    [Header("Runtime")]
    [SerializeField] private UnitBehaviour currentBehaviour;

    private UnitWander unitWander;
    private UnitDialogue unitDialogue;
    private UnitFollow unitFollow;

    Transform IInteractable.Transform => transform;

    private void Awake()
    {
        unitWander = GetComponent<UnitWander>();
        unitDialogue = GetComponent<UnitDialogue>();
        unitFollow = GetComponent<UnitFollow>();

        currentBehaviour = unitWander;
        currentBehaviour.OnBehaviourComplete += CurrentBehaviour_OnBehaviourComplete;
    }

    private void Start()
    {
        currentBehaviour.StartBehaviour();
    }

    public void SetState(FungalState state)
    {
        currentBehaviour.OnBehaviourComplete -= CurrentBehaviour_OnBehaviourComplete;
        currentBehaviour.StopBehaviour();

        currentBehaviour = state switch
        {
            FungalState.WANDER => unitWander,
            FungalState.DIALOGUE => unitDialogue,
            FungalState.FOLLOW => unitFollow,
            _ => unitWander,
        };

        currentBehaviour.StartBehaviour();
        currentBehaviour.OnBehaviourComplete += CurrentBehaviour_OnBehaviourComplete;
    }

    private void CurrentBehaviour_OnBehaviourComplete()
    {
        SetState(FungalState.WANDER);
    }

    public void SetDestination(Vector3 destination, Vector3 direction)
    {
        transform.position = destination;
        transform.forward = direction;
    }

    void IInteractable.OnSelect()
    {
        SetState(FungalState.DIALOGUE);
    }
}
