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

    public bool IsAtDestination => unitDestination.IsAtDestination;

    private UnitDestination unitDestination;
    private UnitFollow unitFollow;
    private Animator animator;
    private CinemachineVirtualCamera virtualCamera;

    protected override void Awake()
    {
        base.Awake();

        unitDestination = GetComponent<UnitDestination>();
        unitFollow = GetComponent<UnitFollow>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public override void Initialize(Unit data)
    {
        base.Initialize(data);
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

    public void Focus()
    {
        virtualCamera.Priority = 11;
    }

    public void Unfocus()
    {
        virtualCamera.Priority = 0;
    }

    public override void OnProximityChanged(bool value)
    {
        base.OnProximityChanged(value);
        chatIcon.SetActive(value);
    }
}
