using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoveController : MonoBehaviour
{
    [SerializeField] private MoveType type;
    [SerializeField] private float speed = 2f;
    [SerializeField] private PositionAnchor positionAnchor;
    [SerializeField] private Animator animator;
    [SerializeField] private bool startIdle;
    [SerializeField] private float minIdleDuration = 2f;
    [SerializeField] private float maxIdleDuration = 5f;

    public enum MoveType
    {
        STOP,
        TARGET,
        POSITION,
        DIRECTION,
        RANDOM,
    }

    private Transform target;
    private Vector3 position;
    private Vector3 direction;
    private Coroutine positionReachedRoutine;
    private bool isIdle;
    private float idleTimer;

    private const float TARGET_DISTANCE_THRESHOLD = 2f;
    private const float POSITION_DISTANCE_THRESHOLD = 0.1f;

    public float Speed => speed;

    public bool IsAtDestination => type switch
    {
        MoveType.POSITION => Vector3.Distance(transform.position, position) < POSITION_DISTANCE_THRESHOLD,
        MoveType.RANDOM => Vector3.Distance(transform.position, position) < POSITION_DISTANCE_THRESHOLD,
        MoveType.TARGET => Vector3.Distance(transform.position, target.position) < TARGET_DISTANCE_THRESHOLD,
        _ => false,
    };

    public event UnityAction OnStart;
    public event UnityAction OnEnd;
    public event UnityAction<Vector3> OnUpdate;

    private void Update()
    {
        switch (type)
        {
            case MoveType.DIRECTION:
                MoveInDirection(direction);
                break;
            case MoveType.TARGET:
                SetLookTarget(target);
                MoveToPosition(target.position);
                break;
            case MoveType.POSITION:
                MoveToPosition(position);
                break;
            case MoveType.RANDOM:
                if (isIdle) UpdateIdle();
                else MoveToPosition(position);
                break;
        }
    }

    #region Initialize Movement Type
    public void SetTarget(Transform target)
    {
        this.target = target;
        type = MoveType.TARGET;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
        type = MoveType.DIRECTION;
    }

    public void SetPosition(Vector3 position, UnityAction onComplete = null)
    {
        type = MoveType.POSITION;
        StartPositionMovement(position, onComplete);
    }

    public void StartRandomMovement()
    {
        type = MoveType.RANDOM;
        if (startIdle) StartIdle();
        else StopIdle();
    }

    private void StartIdle()
    {
        idleTimer = UnityEngine.Random.Range(0f, maxIdleDuration - minIdleDuration);
        isIdle = true;
    }

    private void StopIdle()
    {
        idleTimer = maxIdleDuration;
        isIdle = false;
        StartPositionMovement(positionAnchor.Position, StartIdle);
    }

    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > maxIdleDuration) StopIdle();
    }

    public void Stop()
    {
        type = MoveType.STOP;
    }
    #endregion

    #region Initialization
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void SetBounds(Collider collider)
    {
        positionAnchor.Bounds = collider;
    }
    #endregion

    #region Mutators
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
    #endregion

    private void StartPositionMovement(Vector3 position, UnityAction onComplete = null)
    {
        if (animator) animator.SetBool("isMoving", true);

        this.position = position;

        OnStart?.Invoke();

        if (positionReachedRoutine != null) StopCoroutine(positionReachedRoutine);
        positionReachedRoutine = StartCoroutine(WaitUntilDestinationReached(onComplete));
    }
 
    private IEnumerator WaitUntilDestinationReached(UnityAction onComplete)
    {
        yield return new WaitUntil(() => IsAtDestination);
        onComplete?.Invoke();
        OnEnd?.Invoke();

        if (animator) animator.SetBool("isMoving", false);

    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        if (!IsAtDestination)
        {
            var direction = targetPosition - transform.position;
            MoveInDirection(direction.normalized);
        }
    }

    private void MoveInDirection(Vector3 direction)
    {
        direction.y = 0;

        transform.position += speed * Time.deltaTime * direction;
        if (direction.magnitude > 0) transform.forward = direction;

        OnUpdate?.Invoke(direction);
    }
}
