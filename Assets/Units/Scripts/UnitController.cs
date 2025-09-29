using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour, IInteractable
{
    [Header("Unit References")]
    [SerializeField] protected Transform renderRoot;
    [SerializeField] private UnitBehaviour defaultBehaviour;
    [SerializeField] private UnitBehaviour selectBehaviour;

    [Header("Runtime")]
    [SerializeField] private UnitInstance instance;
    [SerializeField] private UnitBehaviour currentBehaviour;
    [SerializeField] private Vector3 targetLookPosition;
    [SerializeField] private Transform target;

    private CinemachineVirtualCamera virtualCamera;
    private UnitDestination unitDestination;
    protected UnitDestination UnitDestination => unitDestination;

    public UnitInstance Instance => instance;
    protected Transform RenderRoot => renderRoot;
    public bool IsDefaultBehaviour => currentBehaviour == defaultBehaviour;

    Transform ITarget.Transform => transform;

    public virtual Color Color { get; set; }

    public bool IsAtDestination => unitDestination.IsAtDestination;

    public event UnityAction OnInitialized;
    public event UnityAction OnBehaviourChanged;

    protected virtual void Awake()
    {
        targetLookPosition = transform.position;

        var allBehaviours = GetComponents<UnitBehaviour>();
        foreach(var behaviour in allBehaviours)
        {
            behaviour.OnBehaviourRequest += () => ApplyBehaviour(behaviour);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        unitDestination = GetComponent<UnitDestination>();
    }

    protected virtual void Start()
    {
        SetDefaultBehaviour();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Update()
    {
        if (target) targetLookPosition = target.transform.position;

        Vector3 direction = targetLookPosition - renderRoot.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f) // make sure it's not zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            renderRoot.rotation = Quaternion.Slerp(
                renderRoot.rotation,
                targetRotation,
                Time.deltaTime * 5f // rotation speed factor
            );
        }
    }

    protected void ApplyBehaviour(UnitBehaviour behaviour)
    {
        if (currentBehaviour)
        {
            currentBehaviour.OnBehaviourComplete -= SetDefaultBehaviour;
            currentBehaviour.StopBehaviour();
        }

        currentBehaviour = behaviour;

        if (currentBehaviour)
        {
            currentBehaviour.OnBehaviourComplete += SetDefaultBehaviour;
            currentBehaviour.StartBehaviour();
        }

        OnBehaviourChanged?.Invoke();
    }

    public virtual void Initialize(UnitInstance instance)
    {
        this.instance = instance;
        name = "Unit Controller - " + instance.Data.Name;
        OnInitialized?.Invoke();
    }

    public void SetLookPosition(Vector3 targetPosition)
    {
        targetLookPosition = targetPosition;
    }

    public void SetDestination(Vector3 destination)
    {
        unitDestination.SetDestination(destination);
        ApplyBehaviour(unitDestination);
    }

    public void SetLookTarget(Transform target)
    {
        this.target = target;
    }

    public void SetDefaultBehaviour()
    {
        ApplyBehaviour(defaultBehaviour);
    }

    public void SetBehaviour(UnitBehaviour behaviour)
    {
        ApplyBehaviour(behaviour);
    }

    void IInteractable.Select()
    {
        ApplyBehaviour(selectBehaviour);
    }

    public virtual void OnProximityChanged(bool value)
    {
    }

    public void Focus()
    {
        if (virtualCamera) virtualCamera.Priority = 11;
    }

    public void Unfocus()
    {
        if (virtualCamera) virtualCamera.Priority = 0;
    }
}
