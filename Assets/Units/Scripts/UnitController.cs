using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour, IInteractable, INoteTarget
{
    [Header("Unit References")]
    [SerializeField] private Transform renderRoot;
    [SerializeField] private UnitBehaviour defaultBehaviour;
    [SerializeField] private UnitBehaviour selectBehaviour;
    [SerializeField] private int emissionStep;

    [Header("Runtime")]
    [SerializeField] private Unit data;
    [SerializeField] private UnitBehaviour currentBehaviour;
    [SerializeField] private Vector3 targetLookPosition;

    public Unit Data => data;
    protected Transform RenderRoot => renderRoot;
    public bool IsDefaultBehaviour => currentBehaviour == defaultBehaviour;

    Transform ITarget.Transform => transform;

    int INoteTarget.EmissionStep => emissionStep;

    public event UnityAction OnBehaviourChanged;

    protected virtual void Awake()
    {
        targetLookPosition = transform.position;

        var allBehaviours = GetComponents<UnitBehaviour>();
        foreach(var behaviour in allBehaviours)
        {
            behaviour.OnBehaviourRequest += () => ApplyBehaviour(behaviour);
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


    public virtual void Initialize(Unit data)
    {
        this.data = data;
        name = "Unit Controller - " + data.name;
        renderRoot = Instantiate(data.Prefab, transform).transform;
    }

    public void SetLookPosition(Vector3 targetPosition)
    {
        targetLookPosition = targetPosition;
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

    void INoteTarget.OnHit()
    {
    }
}
