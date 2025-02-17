using UnityEngine;

public class Movement : MonoBehaviour
{
    public enum MovementType
    {
        None,
        FollowTarget,
        MoveInDirection,
        MoveToPosition
    }

    private MovementType currentMovement = MovementType.None;
    private Transform target; // Target to follow
    private Vector3 moveDirection; // Directional movement
    private Vector3 targetPosition; // Fixed position movement
    [SerializeField] private Vector3 followOffset;

    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private void Update()
    {
        switch (currentMovement)
        {
            case MovementType.FollowTarget:
                FollowTarget();
                break;
            case MovementType.MoveInDirection:
                MoveInDirection();
                break;
            case MovementType.MoveToPosition:
                MoveToPosition();
                break;
        }
    }

    public void SetFollowOffset(Vector3 followOffset)
    {
        this.followOffset = followOffset;
    }

    /// <summary>
    /// Makes the transform follow a target smoothly.
    /// </summary>
    public void SetFollow(Transform newTarget)
    {
        target = newTarget;
        currentMovement = MovementType.FollowTarget;
    }

    private void FollowTarget()
    {
        if (target == null)
        {
            StopMovement();
            return;
        }

        transform.position = Vector3.Lerp(transform.position, target.position + followOffset, Time.deltaTime * followSpeed);
    }

    /// <summary>
    /// Moves the transform in a given direction at a constant speed.
    /// </summary>
    public void SetMove(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        currentMovement = MovementType.MoveInDirection;
    }

    private void MoveInDirection()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Moves the transform toward a fixed position with smoothing.
    /// </summary>
    public void SetMoveTo(Vector3 position, float stopThreshold = 0.1f)
    {
        targetPosition = position;
        stopDistance = stopThreshold;
        currentMovement = MovementType.MoveToPosition;
    }

    private void MoveToPosition()
    {
        var direction = targetPosition - transform.position;
        transform.position += moveSpeed * Time.deltaTime * direction.normalized;

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            StopMovement();
        }
    }

    /// <summary>
    /// Stops all movement.
    /// </summary>
    public void StopMovement()
    {
        currentMovement = MovementType.None;
    }
}
