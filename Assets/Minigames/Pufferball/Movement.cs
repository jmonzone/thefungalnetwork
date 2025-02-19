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
    [SerializeField] private float moveSpeed = 5f;

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
    private Vector3 circleCenter;
    private float angle;
    private bool reverseDirection;

    public event UnityAction OnDestinationReached;

    private void Awake()
    {
        circleCenter = transform.position;
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
        moveSpeed = speed;
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
        transform.position = Vector3.MoveTowards(transform.position, target.position + followOffset, Time.deltaTime * moveSpeed);
    }

    // Directional Movement
    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        movementType = MovementType.DIRECTIONAL;
    }

    private void MoveInDirection()
    {
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
        UpdateLookDirection(moveDirection);
    }

    // Positional Movement
    public void SetTargetPosition(Vector3 position, float stopThreshold = 0.1f)
    {
        targetPosition = position;
        stopDistance = stopThreshold;
        movementType = MovementType.POSITIONAL;
    }

    private void MoveToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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

        var targetPosition = circleCenter + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * circleRadius;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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
