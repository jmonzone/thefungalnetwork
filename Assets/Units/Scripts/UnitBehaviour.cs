using UnityEngine;
using UnityEngine.Events;

public abstract class UnitBehaviour : MonoBehaviour
{
    protected UnitController Unit { get; private set; }

    public event UnityAction OnBehaviourComplete;

    protected virtual void Awake()
    {
        Unit = GetComponent<UnitController>();
    }

    public abstract void StartBehaviour();

    public virtual void StopBehaviour()
    {
        OnBehaviourComplete?.Invoke();
    }
}
