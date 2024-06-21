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

    private Func<Vector3> getTargetPosition;

    private Vector3 direction;
    private Coroutine positionReachedRoutine;
    private bool isIdle;
    private float idleTimer;

    private const float TARGET_DISTANCE_THRESHOLD = 2f;
    private const float POSITION_DISTANCE_THRESHOLD = 0.1f;

    public float Speed => speed;

    public event UnityAction OnStart;
    public event UnityAction OnEnd;
    public event UnityAction<Vector3> OnUpdate;

    #region Public Methods
    public bool IsAtDestination => type switch
    {
        MoveType.POSITION => Vector3.Distance(transform.position, getTargetPosition()) < POSITION_DISTANCE_THRESHOLD,
        MoveType.RANDOM => Vector3.Distance(transform.position, getTargetPosition()) < POSITION_DISTANCE_THRESHOLD,
        MoveType.TARGET => Vector3.Distance(transform.position, getTargetPosition()) < TARGET_DISTANCE_THRESHOLD,
        _ => false,
    };

    public void SetTarget(Transform target)
    {
        getTargetPosition = () => target.position;
        SetState(MoveType.TARGET);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
        SetState(MoveType.DIRECTION);
    }

    public void SetPosition(Vector3 position, UnityAction onComplete = null)
    {
        SetState(MoveType.POSITION);
        StartPositionMovement(position, onComplete);
    }

    public void StartRandomMovement()
    {
        SetState(MoveType.RANDOM);
        
    }

    public void Stop()
    {
        SetState(MoveType.STOP);
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
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
    #endregion

    private void SetState(MoveType type)
    {
        this.type = type;
        isIdle = type == MoveType.RANDOM && startIdle;
        switch (type)
        {
            case MoveType.RANDOM:
                if (startIdle) StartIdle();
                else StopIdle();
                break;
        }
    }

    private void Update()
    {
        switch (type)
        {
            case MoveType.DIRECTION:
                UpdateDirection(direction);
                break;
            case MoveType.TARGET:
            case MoveType.POSITION:
                UpdatePosition();
                break;
            case MoveType.RANDOM:
                if (isIdle) UpdateIdle();
                else UpdatePosition();
                break;
        }
    }

    private void UpdatePosition()
    {
        if (IsAtDestination) return;

        var direction = getTargetPosition() - transform.position;
        UpdateDirection(direction.normalized);
    }

    private void UpdateDirection(Vector3 direction)
    {
        direction.y = 0;

        transform.position += speed * Time.deltaTime * direction;
        if (direction != Vector3.zero) transform.forward = direction;

        OnUpdate?.Invoke(direction);
    }


    private void StartPositionMovement(Vector3 position, UnityAction onComplete = null)
    {
        if (animator) animator.SetBool("isMoving", true);

        getTargetPosition = () => position;

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
}
