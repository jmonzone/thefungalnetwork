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
    [SerializeField] private bool useSmoothDirection = true;
    [SerializeField] private float directionSmoothTime = 0.1f;  // Tweak for more/less drift
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform lookTransform;
    [SerializeField] private Transform scaleTransform;
    private Vector3 originalScale;

    public Transform ScaleTransform => scaleTransform;
    private Vector3 inputDirection;
    private Vector3 currentDirection = Vector3.zero;
    private Vector3 directionSmoothVelocity;

    private float modifier = 1f;

    public float CalculatedSpeed => baseSpeed * modifier;
    public float SpeedDelta => CalculatedSpeed * Time.deltaTime;

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
        originalScale = scaleTransform.localScale;
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
        var previousType = this.type;
        this.type = type;

        if (previousType != type)
        {
            //Debug.Log($"{name}.Movement.OnTypeChanged {previousType} -> {type}");
            OnTypeChanged?.Invoke();
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
        //Debug.Log($"Follow {newTarget.name}");

        target = newTarget;
        SetType(MovementType.FOLLOW);
    }

    private void FollowTarget()
    {
        if (target == null) return;

        // Rotate the follow offset based on the target's rotation
        Vector3 rotatedOffset = target.rotation * followOffset;

        // Update direction and position
        UpdateLookDirection(target.position + rotatedOffset - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target.position + rotatedOffset, SpeedDelta);

    }

    // Directional Movement
    public void SetDirection(Vector3 direction)
    {
        if (direction.magnitude > 0.01f)
        {
            this.inputDirection = direction;
            SetType(MovementType.DIRECTIONAL);
        }
        else
        {
            Stop();
        }
    }

    private void MoveInDirection()
    {
        if (useSmoothDirection)
        {
            // Smoothly interpolate the currentDirection towards the target direction
            currentDirection = Vector3.SmoothDamp(currentDirection, inputDirection, ref directionSmoothVelocity, directionSmoothTime);
        }
        else
        {
            currentDirection = inputDirection;
        }

        if (currentDirection.magnitude > 0.01f)
        {
            transform.position += SpeedDelta * currentDirection;
        }
        else
        {
            Stop();
        }

        UpdateLookDirection(currentDirection);
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

    public bool IsAtDestination => Vector3.Distance(transform.position, targetPosition) <= stopDistance;

    private void MoveToPosition()
    {
        UpdateLookDirection(targetPosition - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, SpeedDelta);

        if (IsAtDestination)
        {
            Stop();
            OnDestinationReached?.Invoke();
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
        trajectoryTimeElapsed += SpeedDelta;
        float progress = Mathf.Clamp01(trajectoryTimeElapsed / trajectoryDuration);

        // Calculate the position on the XZ plane with constant speed
        Vector3 currentPosition = Vector3.Lerp(trajectoryStartPosition, trajectoryEndPosition, progress);

        // Calculate the height using a simple curve (parabola-like trajectory)
        float heightOffset = Mathf.Sin(progress * Mathf.PI) * trajectoryHeight;

        targetPosition = new Vector3(currentPosition.x, heightOffset, currentPosition.z);

        var lookDirection = targetPosition - transform.position;
        UpdateLookDirection(lookDirection);

        // Set the final position with calculated height
        transform.position = targetPosition;

        // If the movement is complete, stop the movement
        if (progress >= 1f)
        {
            Stop();

            //Debug.Log("Stopping, updating look direction " + lookDirection);
            lookDirection.y = 0;
            UpdateLookDirection(lookDirection.normalized, force: true);

            OnDestinationReached?.Invoke();
        }
    }

    private void UpdateLookDirection(Vector3 direction, bool force = false)
    {
        var lookDirection = direction;

        if (lookDirection != Vector3.zero)
        {
            // Smoothly rotate using Slerp (Spherical Linear Interpolation)
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            if (force) lookTransform.rotation = targetRotation;
            else lookTransform.rotation = Quaternion.Slerp(lookTransform.rotation, targetRotation, rotationSpeed * SpeedDelta);
        }
    }

    // Idle State (Stop Movement)
    public void Stop()
    {
        SetType(MovementType.IDLE);
    }

    private float scaleFactor = 1f;

    public IEnumerator ScaleOverTime(float duration, float endScale)
    {
        yield return ScaleOverTime(duration, scaleFactor, endScale);
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

    public void SetScaleFactor(float factor)
    {
        scaleFactor = factor;
        SetTargetScale(originalScale * factor);
    }

    public void SetTargetScale(Vector3 targetScale)
    {
        // Final scale adjustment
        float finalX = lockX ? lockedXValue : targetScale.x;
        float finalY = lockY ? lockedYValue : targetScale.y;
        float finalZ = lockZ ? lockedZValue : targetScale.z;

        scaleTransform.localScale = new Vector3(finalX, finalY, finalZ);
    }

    public void ResetScaleFactor()
    {
        SetTargetScale(originalScale);
    }

}
