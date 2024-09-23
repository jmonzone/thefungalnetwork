using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool faceForward = true;
    [SerializeField] private bool lockXZ = false;
    [SerializeField] private bool useDrag = false;
    [SerializeField] private PositionAnchor positionAnchor;
    
    [Header("Target Movement")]
    [SerializeField] private float distanceThreshold = 2f;

    [Header("Random Movement")]
    [SerializeField] private bool startIdle;
    [SerializeField] private float minIdleDuration = 2f;
    [SerializeField] private float maxIdleDuration = 5f;

    [Header("Radial Movement")]
    [SerializeField] private float radius = 4f;
    [SerializeField] private float radialSpeed = 0.25f;

    private enum MovementType
    {
        TARGET,
        DIRECTION,
        POSITION,
        RADIAL
    }

    private MovementType type = MovementType.POSITION;
    private Coroutine positionReachedRoutine;
    private bool isIdle;
    private float idleTimer;

    private Transform target;
    private Vector3 position;
    private Vector3 origin;
    private float angle;

    public Vector3 Direction { get; private set; }

    public float Speed => speed;
    public float DistanceThreshold => distanceThreshold;
    public bool FaceForward => faceForward;

    public bool IsAtDestination => Vector3.Distance(transform.position, TargetPosition) < 0.1f;

    public Vector3 TargetPosition => type switch
    {
        MovementType.TARGET => GetTargetPosition(),
        MovementType.DIRECTION => transform.position + Direction,
        MovementType.RADIAL => GetRadialPosition(),
        _ => position,
    };

    #region Public Methods
    public void SetTarget(Transform target)
    {
        this.target = target;
        SetType(MovementType.TARGET);
    }

    public void SetDirection(Vector3 direction)
    {
        this.Direction = direction;
        SetType(MovementType.DIRECTION);
    }

    public void SetPosition(Vector3 position, UnityAction onComplete = null)
    {
        this.position = position;
        SetType(MovementType.POSITION);
        positionReachedRoutine = StartCoroutine(WaitUntilDestinationReached(onComplete));
    }

    public void StartRadialMovement(Vector3 origin)
    {
        this.origin = origin;
        SetType(MovementType.RADIAL);
    }

    private void SetType(MovementType type)
    {
        this.type = type;
        isIdle = false;
        if (positionReachedRoutine != null) StopCoroutine(positionReachedRoutine);
    }

    public void StartRandomMovement()
    {
        if (startIdle) StartIdle();
        else SetPosition(positionAnchor.Position, StartIdle);
    }

    public void Stop()
    {
        SetPosition(transform.position);
    }

    public void SetBounds(Collider collider)
    {
        positionAnchor.Bounds = collider;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetLookTarget(Transform target)
    {
        var direction = target.position - transform.position;
        direction.y = 0;
        transform.forward = direction;
    }

    public void SetDistanceThreshold(float distanceThreshold)
    {
        this.distanceThreshold = distanceThreshold;
    }
    #endregion

    private void Awake()
    {
        position = transform.position;
    }

    private void Update()
    {
        if (isIdle) UpdateIdle();
        else UpdatePosition();
    }

    private Vector3 GetTargetPosition()
    {
        var direction = (target.position - transform.position);
        return target.position - direction.normalized * distanceThreshold;
    }

    private Vector3 GetRadialPosition()
    {
        angle += Time.deltaTime * radialSpeed;
        var x = Mathf.Cos(angle);
        var z = Mathf.Sin(angle);
        var direction = new Vector3(x, 0, z) * radius;
        return origin + direction;
    }

    private void UpdatePosition()
    {
        if (IsAtDestination) return;

        Direction = (TargetPosition - transform.position).normalized;


        float angle = Vector3.Angle(transform.forward, Direction);
        if (!faceForward || angle < Mathf.PI) transform.position += speed * Time.deltaTime * Direction;

        if (useDrag)
        {
            if (speed > 0.05f) speed *= 1 - Time.deltaTime * 0.1f;
            else speed = 0;
        }

        if (faceForward && Direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Direction, Vector3.up);
            if (lockXZ) targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Keep only y-axis rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 500 * Time.deltaTime);
        }

    }

    private IEnumerator WaitUntilDestinationReached(UnityAction onComplete)
    {
        yield return new WaitUntil(() => IsAtDestination);
        onComplete?.Invoke();
    }

    private void StartIdle()
    {
        idleTimer = Random.Range(0f, maxIdleDuration - minIdleDuration);
        isIdle = true;
    }

    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > maxIdleDuration) SetPosition(positionAnchor.Position, StartIdle);
    }
}
