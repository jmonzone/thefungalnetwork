using System.Collections;
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
        RADIAL,
        TRAJECTORY // New movement type
    }

    [SerializeField] private MovementType movementType = MovementType.IDLE;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform lookTransform;

    private Vector3 direction;
    private float modifier = 1f;
    private float Speed => baseSpeed * modifier * Time.deltaTime;

    [Header("Follow Target Settings")]
    [SerializeField] private Vector3 followOffset;
    private Transform target;


    [Header("Move To Position Settings")]
    [SerializeField] private float stopDistance = 0.1f;
    private Vector3 targetPosition;

    [Header("Radial Movement Settings")]
    [SerializeField] private float circleRadius = 5f;
    public Vector3 CircleCenter { get; private set; }

    private float angle;
    private bool reverseDirection;

    [Header("Trajectory Movement Settings")]
    [SerializeField] private float trajectoryHeight = 3f; // Height of the trajectory
    [SerializeField] private float trajectoryDuration = 2f; // Duration of the movement in seconds
    private Vector3 trajectoryStartPosition;
    private Vector3 trajectoryEndPosition;
    private float trajectoryTimeElapsed;

    private Vector3 originalScale;

    public event UnityAction OnDestinationReached;

    private void Awake()
    {
        originalScale = transform.GetChild(0).localScale;

        CircleCenter = transform.position;
        if (!lookTransform) lookTransform = transform;
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
            case MovementType.TRAJECTORY:
                MoveAlongTrajectory();
                break;
            case MovementType.IDLE:
                var lookDirection = lookTransform.forward;
                lookDirection.y = 0;
                UpdateLookDirection(lookDirection);
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
        UpdateLookDirection(target.position + followOffset - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target.position + followOffset, Speed);
    }

    // Directional Movement
    public void SetDirection(Vector3 direction, float speed)
    {
        this.direction = direction.normalized;
        baseSpeed = speed;
        movementType = MovementType.DIRECTIONAL;
    }

    private void MoveInDirection()
    {
        UpdateLookDirection(direction);
        transform.position += Speed * direction;
        UpdateLookDirection(direction);
    }

    // Positional Movement
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = GetValidTargetPosition(position);
        movementType = MovementType.POSITIONAL;
    }

    private Vector3 GetValidTargetPosition(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;
        origin.y = targetPosition.y;

        Vector3 direction = (targetPosition - origin).normalized;
        float maxDistance = Vector3.Distance(origin, targetPosition);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, obstacleLayer))
        {
            // If an obstacle is hit, adjust the target position to be just before the obstacle
            return hit.point - direction * stopDistance;
        }

        return targetPosition;
    }

    private void MoveToPosition()
    {
        UpdateLookDirection(targetPosition - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed);

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            OnDestinationReached?.Invoke();
            Stop();
        }
    }

    // Radial Movement
    public void StartRadialMovement(Vector3 center, bool reverse)
    {
        CircleCenter = center;
        reverseDirection = reverse;
        movementType = MovementType.RADIAL;
    }

    private void MoveInCircle()
    {
        angle += reverseDirection ? -Time.deltaTime : Time.deltaTime;
        targetPosition = CircleCenter + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * circleRadius;
        UpdateLookDirection(targetPosition - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed);
    }

    // Trajectory Movement
    public void SetTrajectoryMovement(Vector3 endPosition)
    {
        trajectoryStartPosition = transform.position;
        trajectoryEndPosition = endPosition;
        //trajectoryHeight = height;
        //trajectoryDuration = duration;
        trajectoryTimeElapsed = 0f;
        movementType = MovementType.TRAJECTORY;
    }

    private void MoveAlongTrajectory()
    {
        trajectoryTimeElapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(trajectoryTimeElapsed / trajectoryDuration);

        // Calculate the position on the XZ plane with constant speed
        Vector3 currentPosition = Vector3.Lerp(trajectoryStartPosition, trajectoryEndPosition, progress);

        // Calculate the height using a simple curve (parabola-like trajectory)
        float heightOffset = Mathf.Sin(progress * Mathf.PI) * trajectoryHeight;

        targetPosition = new Vector3(currentPosition.x, heightOffset, currentPosition.z);
        UpdateLookDirection(targetPosition - transform.position);

        // Set the final position with calculated height
        transform.position = targetPosition;

        // If the movement is complete, stop the movement
        if (progress >= 1f)
        {
            Stop();
            OnDestinationReached?.Invoke();
        }
    }

    private void UpdateLookDirection(Vector3 direction)
    {
        var lookDirection = direction;

        if (lookDirection.magnitude > 0)
        {
            // Smoothly rotate using Slerp (Spherical Linear Interpolation)
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            lookTransform.rotation = Quaternion.Slerp(lookTransform.rotation, targetRotation, Time.deltaTime * baseSpeed);
        }
    }

    // Idle State (Stop Movement)
    public void Stop()
    {
        movementType = MovementType.IDLE;
    }

    // Generalized scaling coroutine
    public IEnumerator ScaleOverTime(float duration, float startScale, float endScale)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(startScale, endScale, elapsed / duration);
            SetScaleFactor(scaleFactor);
            yield return null;
        }

        transform.GetChild(0).localScale = originalScale * endScale; // Ensure final scale
    }

    public void SetScaleFactor(float scaleFactor)
    {
        transform.GetChild(0).localScale = originalScale * scaleFactor;
    }

    public void ResetScaleFactor()
    {
        SetScaleFactor(1);
    }

}
