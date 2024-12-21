using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementType type;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool lerpRotation = true;
    [SerializeField] private bool faceForward = true;
    [SerializeField] private bool useRoll = true;
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


    [Header("Jump Movement")]
    [SerializeField] private int maxJumpCount = 1;

    private int jumpCount = 0;
    private Rigidbody rb;

    private enum MovementType
    {
        IDLE,
        POSITION,
        TARGET,
        DIRECTION,
        RADIAL
    }

    private Coroutine positionReachedRoutine;
    private bool isIdle;
    private float idleTimer;

    private Transform target;
    private Vector3 position;
    private Vector3 origin;
    private float angle;

    public Vector3 Direction { get; private set; }

    public float Speed => speed;
    public bool FaceForward => faceForward;

    public bool IsAtDestination => Vector3.Distance(transform.position, TargetPosition) < distanceThreshold;

    public Vector3 TargetPosition => type switch
    {
        MovementType.IDLE => transform.position,
        MovementType.TARGET => GetTargetPosition(),
        MovementType.DIRECTION => transform.position + Direction,
        MovementType.RADIAL => GetRadialPosition(),
        _ => position,
    };

    public event UnityAction OnJump;

    #region Public Methods
    public void SetTarget(Transform target, UnityAction onComplete = null)
    {
        this.target = target;
        SetType(MovementType.TARGET);
        positionReachedRoutine = StartCoroutine(WaitUntilDestinationReached(onComplete));
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
        SetType(MovementType.IDLE);
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

    public bool CanJump => jumpCount < maxJumpCount;

    public void Jump()
    {
        if (!CanJump) return;
        jumpCount++;
        rb.AddForce(Vector3.up * 250f);
        OnJump?.Invoke();
    }

    public void SetMaxJumpCount(int maxJumpCount)
    {
        this.maxJumpCount = maxJumpCount;
    }
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpCount = 0;
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

        if (type == MovementType.POSITION || type == MovementType.TARGET) Direction = (TargetPosition - transform.position).normalized;

        if (Direction != Vector3.zero)
        {
            // make enum flag to prevent conflictions
            if (lerpRotation)
            {
                //todo: clean up this logic
                var lookDirection = Direction;
                if (type == MovementType.TARGET) lookDirection = target.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                if (lockXZ) targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Keep only y-axis rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000 * Time.deltaTime);
                transform.position += speed * Time.deltaTime * transform.forward;
            }
            else if (FaceForward)
            {
                transform.forward = Direction;
                transform.position += speed * Time.deltaTime * transform.forward;
            }
            else if (useRoll)
            {
                rb.velocity = speed * Direction;
            }
        }


        if (useDrag)
        {
            if (speed > 0.05f) speed *= 1 - Time.deltaTime * 0.1f;
            else speed = 0;
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
