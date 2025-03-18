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

    [SerializeField] private MovementType type = MovementType.IDLE;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform lookTransform;
    [SerializeField] private Transform scaleTransform;


    public Transform ScaleTransform => scaleTransform;
    private Vector3 direction;
    private float modifier = 1f;

    public float CalculatedSpeed => baseSpeed * modifier;
    private float SpeedDelta => CalculatedSpeed * Time.deltaTime;

    public MovementType Type => type;

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

    public event UnityAction OnTypeChanged;
    public event UnityAction OnDestinationReached;

    private void Awake()
    {
        CircleCenter = transform.position;

        if (!lookTransform) lookTransform = transform;
        if (!scaleTransform) scaleTransform = transform.GetChild(0);
    }

    private void Update()
    {
        switch (type)
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

    public void SetType(MovementType type)
    {
        this.type = type;
        OnTypeChanged?.Invoke();
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
        SetType(MovementType.FOLLOW);
    }

    private void FollowTarget()
    {
        if (target == null) return;
        UpdateLookDirection(target.position + followOffset - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target.position + followOffset, SpeedDelta);
    }

    // Directional Movement
    public void SetDirection(Vector3 direction, float speed)
    {
        this.direction = direction;
        baseSpeed = speed;
        SetType(MovementType.DIRECTIONAL);
    }

    private void MoveInDirection()
    {
        UpdateLookDirection(direction);
        transform.position += SpeedDelta * direction;
    }

    // Positional Movement
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = GetValidTargetPosition(position);
        SetType(MovementType.POSITIONAL);
    }

    private Vector3 GetValidTargetPosition(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;
        targetPosition.y = origin.y;

        Vector3 direction = (targetPosition - origin).normalized;
        float maxDistance = Vector3.Distance(origin, targetPosition);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, obstacleLayer))
        {
            // If an obstacle is hit, adjust the target position to be just before the obstacle
            var adjustedPosition = hit.point - direction * stopDistance;
            adjustedPosition.y = origin.y;
            return adjustedPosition;
        }

        return targetPosition;
    }

    private void MoveToPosition()
    {
        UpdateLookDirection(targetPosition - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, SpeedDelta);

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
        SetType(MovementType.RADIAL);
    }

    private void MoveInCircle()
    {
        angle += reverseDirection ? -Time.deltaTime : Time.deltaTime;
        targetPosition = CircleCenter + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * circleRadius;
        UpdateLookDirection(targetPosition - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, SpeedDelta);
    }

    // Trajectory Movement
    public void SetTrajectoryMovement(Vector3 endPosition)
    {
        trajectoryStartPosition = transform.position;
        trajectoryEndPosition = endPosition;
        //trajectoryHeight = height;
        //trajectoryDuration = duration;
        trajectoryTimeElapsed = 0f;
        SetType(MovementType.TRAJECTORY);
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

        if (lookDirection != Vector3.zero)
        {
            // Smoothly rotate using Slerp (Spherical Linear Interpolation)
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            lookTransform.rotation = Quaternion.Slerp(lookTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Idle State (Stop Movement)
    public void Stop()
    {
        SetType(MovementType.IDLE);
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

        SetScaleFactor(endScale);
    }

    [SerializeField] private bool lockX = false;  // Lock X axis
    [SerializeField] private bool lockY = false;  // Lock Y axis
    [SerializeField] private bool lockZ = false;  // Lock Z axis

    [SerializeField] private float lockedXValue = 1f;  // Locked value for X
    [SerializeField] private float lockedYValue = 1f;  // Locked value for Y
    [SerializeField] private float lockedZValue = 1f;  // Locked value for Z

    public void SetScaleFactor(float scaleFactor)
    {
        float x = lockX ? lockedXValue : scaleFactor;
        float y = lockY ? lockedYValue : scaleFactor;
        float z = lockZ ? lockedZValue : scaleFactor;

        scaleTransform.localScale = new Vector3(x, y, z);
    }

    public void ResetScaleFactor()
    {
        SetScaleFactor(1);
    }

}
