using UnityEngine;
using UnityEngine.Events;

public abstract class UnitBehaviour : MonoBehaviour
{
    [SerializeField] private bool isActive;

    public UnitController Unit { get; private set; }
    public UnitInstance Instance => Unit.Instance;
    public bool IsActive => isActive;

    public event UnityAction OnBehaviourRequest;
    public event UnityAction OnBehaviourComplete;

    protected virtual void Awake()
    {
        Unit = GetComponent<UnitController>();
        Unit.OnInitialized += OnInitialized;
    }

    protected virtual void OnInitialized()
    {
    }

    protected virtual void Update()
    {

    }

    public virtual void StartBehaviour()
    {
        if (!isActive)
        {
            isActive = true;
            OnBehaviourStart();
        }
    }

    protected abstract void OnBehaviourStart();

    public virtual void StopBehaviour()
    {
        if (isActive)
        {
            isActive = false;
            OnBehaviourComplete?.Invoke();
        }
    }

    protected void InvokeOnBehaviourRequest()
    {
        OnBehaviourRequest?.Invoke();
    }
}
