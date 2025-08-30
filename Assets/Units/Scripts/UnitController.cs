using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour
{
    [Header("Unit References")]
    [SerializeField] private Unit data;
    [SerializeField] private Transform renderRoot;

    public Unit Data => data;
    protected Transform RenderRoot => renderRoot;

    public event UnityAction OnInitialized;
    public event UnityAction OnSelected;
    public event UnityAction OnUnselect;

    public void Initialize(Unit data)
    {
        this.data = data;
        name = "Unit Controller - " + data.name;
        renderRoot = Instantiate(data.Prefab, transform).transform;
        OnInitialized?.Invoke();
    }

    public void Select()
    {
        OnSelected?.Invoke();
    }

    public void Unselect()
    {
        OnUnselect?.Invoke();
    }

    public void LookAt(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero) renderRoot.forward = direction;
    }
}
