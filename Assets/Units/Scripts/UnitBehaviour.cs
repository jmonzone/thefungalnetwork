using UnityEngine;
using UnityEngine.Events;

public abstract class UnitBehaviour : MonoBehaviour
{
    [SerializeField] private bool isActive;
    [SerializeField] private bool isPaused;

    public UnitController Controller { get; private set; }
    public UnitInstance Instance => Controller.Instance;
    public bool IsActive => isActive;

    public event UnityAction OnBehaviourRequest;
    public event UnityAction OnBehaviourComplete;

    protected virtual void Awake()
    {
        Controller = GetComponent<UnitController>();
        Controller.OnInitialized += OnInitialized;
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

    public virtual void PauseBehaviour()
    {
        isPaused = true;
    }

    public virtual void UnpauseBehaviour()
    {
        isPaused = false;
    }

    protected void InvokeOnBehaviourRequest()
    {
        OnBehaviourRequest?.Invoke();
    }
}
