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

    Transform IInteractable.Transform => transform;

    public event UnityAction OnInitialized;

    protected virtual void Awake()
    {
        targetLookPosition = transform.position;
    }

    protected virtual void Start()
    {
        SetBehaviour(defaultBehaviour);
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
            currentBehaviour.OnBehaviourComplete -= CurrentBehaviour_OnBehaviourComplete;
            currentBehaviour.StopBehaviour();
        }

        currentBehaviour = behaviour;

        if (currentBehaviour)
        {
            currentBehaviour.StartBehaviour();
            currentBehaviour.OnBehaviourComplete += CurrentBehaviour_OnBehaviourComplete;
        }
    }

    private void CurrentBehaviour_OnBehaviourComplete()
    {
        SetBehaviour(defaultBehaviour);
    }

    public void Initialize(Unit data)
    {
        this.data = data;
        name = "Unit Controller - " + data.name;
        renderRoot = Instantiate(data.Prefab, transform).transform;
        OnInitialized?.Invoke();
    }

    public void LookAt(Vector3 targetPosition)
    {
        targetLookPosition = targetPosition;
    }

    void IInteractable.OnSelect()
    {
        SetBehaviour(selectBehaviour);
    }
}
