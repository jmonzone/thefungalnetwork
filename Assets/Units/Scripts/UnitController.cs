using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour, IInteractable
{
    [Header("Unit References")]
    [SerializeField] private Transform renderRoot;
    [SerializeField] private UnitBehaviour defaultBehaviour;
    [SerializeField] private UnitBehaviour selectBehaviour;

    [Header("Runtime")]
    [SerializeField] private Unit data;
    [SerializeField] private UnitBehaviour currentBehaviour;
    [SerializeField] private Vector3 targetLookPosition;

    public Unit Data => data;
    protected Transform RenderRoot => renderRoot;
    public bool IsDefaultBehaviour => currentBehaviour == defaultBehaviour;

    Transform IInteractable.Transform => transform;

    public event UnityAction OnInitialized;
    public event UnityAction OnBehaviourChanged;

    protected virtual void Awake()
    {
        targetLookPosition = transform.position;

        var allBehaviours = GetComponents<UnitBehaviour>();
        foreach(var behaviour in allBehaviours)
        {
            behaviour.OnBehaviourRequest += () => SetBehaviour(behaviour);
        }
    }

    protected virtual void Start()
    {
        SetDefaultBehaviour();
    }

    protected virtual void Update()
    {
        Vector3 direction = targetLookPosition - transform.position;
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

    protected void SetBehaviour(UnitBehaviour behaviour)
    {
        if (currentBehaviour)
        {
            currentBehaviour.OnBehaviourComplete -= SetDefaultBehaviour;
            if (currentBehaviour.IsActive) currentBehaviour.StopBehaviour();
        }

        currentBehaviour = behaviour;
        OnBehaviourChanged?.Invoke();

        if (currentBehaviour)
        {
            currentBehaviour.StartBehaviour();
            currentBehaviour.OnBehaviourComplete += SetDefaultBehaviour;
        }
    }

    public void SetDefaultBehaviour()
    {
        SetBehaviour(defaultBehaviour);
    }

    public virtual void Initialize(Unit data)
    {
        this.data = data;
        name = "Unit Controller - " + data.name;
        renderRoot = Instantiate(data.Prefab, transform).transform;
        OnInitialized?.Invoke();
    }

    public void SetLookPosition(Vector3 targetPosition)
    {
        targetLookPosition = targetPosition;
    }

    void IInteractable.Select()
    {
        SetBehaviour(selectBehaviour);
    }
}
