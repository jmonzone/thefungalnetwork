using Cinemachine;
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
    [Header("Fungal References")]
    [SerializeField] private GameObject chatIcon;
    [SerializeField] private DialogueReference dialogueReference;

    public bool IsAtDestination => unitDestination.IsAtDestination;

    private UnitDestination unitDestination;
    private UnitFollow unitFollow;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        unitDestination = GetComponent<UnitDestination>();
        unitFollow = GetComponent<UnitFollow>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        dialogueReference.OnIsActiveChanged += DialogueReference_OnIsActiveChanged;
    }

    private void DialogueReference_OnIsActiveChanged()
    {
        OnProximityChanged(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        dialogueReference.OnIsActiveChanged -= DialogueReference_OnIsActiveChanged;
    }

    public override void Initialize(UnitInstance instance)
    {
        base.Initialize(instance);
        animator = GetComponentInChildren<Animator>();
    }

    public void SetDestination(Vector3 destination)
    {
        unitDestination.SetDestination(destination);
        ApplyBehaviour(unitDestination);
    }

    public void SetTarget(Transform target)
    {
        unitFollow.SetTarget(target);
        ApplyBehaviour(unitFollow);
    }

    public void TriggerDeath()
    {
        animator.Play("Death");
    }

    public void TriggerRespawn()
    {
        animator.SetTrigger("Respawn");
    }

    public override void OnProximityChanged(bool value)
    {
        base.OnProximityChanged(value);
        chatIcon.SetActive(!dialogueReference.IsActive && value);
    }
}
