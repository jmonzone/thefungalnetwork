using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PlayerReference : ScriptableObject
{
    [Header("Runtime")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private UnitController targetUnit;
    [SerializeField] private bool isAtDestination;

    public PlayerController Player => player;
    public Vector3 TargetPosition => targetPosition;
    public UnitController TargetUnit => targetUnit;
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
        SetTargetUnit(null);
    }

    public void SetTargetUnit(UnitController targetUnit)
    {
        this.targetUnit = targetUnit;
        isAtDestination = false;
    }

    public void InvokeOnDestinationReached()
    {
        if (!isAtDestination)
        {
            isAtDestination = true;
            OnDestinationReached?.Invoke();

            targetUnit.Select();
        }
    }
}
