using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    public enum MovementType
    {
        IDLE,
        FOLLOW,
        DIRECTIONAL,
        POSITIONAL,
        RADIAL
    }

    [SerializeField] private MovementType movementType = MovementType.IDLE;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    private float modifier = 1f;
    private float Speed => baseSpeed * modifier * Time.deltaTime;

    [Header("Follow Target Settings")]
    [SerializeField] private Vector3 followOffset;
    private Transform target;

    [Header("Move In Direction Settings")]
    private Vector3 moveDirection;

    [Header("Move To Position Settings")]
    [SerializeField] private float stopDistance = 0.1f;
    private Vector3 targetPosition;

    [Header("Radial Movement Settings")]
    [SerializeField] private float circleRadius = 5f;
    public Vector3 CircleCenter { get; private set; }

    private float angle;
    private bool reverseDirection;

    public event UnityAction OnDestinationReached;

    private void Awake()
    {
        CircleCenter = transform.position;
    }

    private void Update()
    {
        switch (movementType)
        {
            case MovementType.FOLLOW:
                FollowTarget();
                break;
            case MovementType.DIRECTIONAL:
                MoveInDirection();
                break;
            case MovementType.POSITIONAL:
                MoveToPosition();
                break;
            case MovementType.RADIAL:
                MoveInCircle();
                break;
        }
    }

    public void SetSpeed(float speed)
    {
        baseSpeed = speed;
    }

    public void SetSpeedModifier(float modifier)
    {
        this.modifier = modifier;
    }

    public void ResetSpeedModifier()
    {
        modifier = 1f;
    }

    // Follow Target Movement
    public void Follow(Transform newTarget)
    {
        target = newTarget;
        movementType = MovementType.FOLLOW;
    }

    private void FollowTarget()
    {
        if (target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, target.position + followOffset, Speed);
    }

    // Directional Movement
    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        baseSpeed = speed;
        movementType = MovementType.DIRECTIONAL;
    }

    private void MoveInDirection()
    {
        transform.position += Speed * moveDirection;
        UpdateLookDirection(moveDirection);
    }

    // Positional Movement
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = GetValidTargetPosition(position);
        movementType = MovementType.POSITIONAL;
    }

    /// <summary>
    /// Performs a raycast and clamps the target position if an obstacle is in the way.
    /// </summary>
    private Vector3 GetValidTargetPosition(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;
        origin.y = targetPosition.y;

        Vector3 direction = (targetPosition - origin).normalized;
        float maxDistance = Vector3.Distance(origin, targetPosition);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, obstacleLayer))
        {
            // If an obstacle is hit, adjust the target position to be just before the obstacle
            return hit.point - direction * stopDistance; // Slightly offset from the obstacle
        }

        return targetPosition; // No obstacle, use original position
    }

    private void MoveToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed);
        UpdateLookDirection(targetPosition - transform.position);

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            OnDestinationReached?.Invoke();
            Stop();
        }
    }

    // Radial Movement
    public void StartRadialMovement(bool reverse)
    {
        reverseDirection = reverse;
        movementType = MovementType.RADIAL;
    }

    private void MoveInCircle()
    {
        angle += reverseDirection ? -Time.deltaTime : Time.deltaTime;

        var targetPosition = CircleCenter + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * circleRadius;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed);
    }

    private void UpdateLookDirection(Vector3 direction)
    {
        var lookDirection = direction;
        lookDirection.y = 0;
        transform.forward = lookDirection;
    }

    // Idle State (Stop Movement)
    public void Stop()
    {
        movementType = MovementType.IDLE;
    }
}
