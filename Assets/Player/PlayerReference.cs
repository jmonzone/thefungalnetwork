using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PlayerReference : ScriptableObject
{
    [Header("Runtime")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private IInteractable targetInteractable;
    [SerializeField] private bool isAtDestination;

    public PlayerController Player => player;
    public Vector3 TargetPosition => targetPosition;
    public IInteractable TargetInteractable => targetInteractable;
    public bool IsAtDestination => isAtDestination;

    public event UnityAction OnDestinationReached;

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        isAtDestination = false;
        SetTargetInteractable(null);
    }

    public void SetTargetInteractable(IInteractable targetInteractable)
    {
        this.targetInteractable = targetInteractable;
        isAtDestination = false;
    }

    public void InvokeOnDestinationReached()
    {
        if (!isAtDestination)
        {
            isAtDestination = true;
            OnDestinationReached?.Invoke();

            targetInteractable.OnSelect();
        }
    }
}
