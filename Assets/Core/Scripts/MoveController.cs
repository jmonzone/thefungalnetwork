using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum MoveState
{
    POSITION,
    DIRECTION,
}

public class MoveController : MonoBehaviour
{
    [SerializeField] private MoveState state;
    [SerializeField] private float speed = 2f;
    [SerializeField] private PositionAnchor positionAnchor;
    [SerializeField] private Animator animator;
    [SerializeField] private bool startIdle;
    [SerializeField] private float minIdleDuration = 2f;
    [SerializeField] private float maxIdleDuration = 5f;

    private Func<Vector3> getTargetPosition;
    private Func<Vector3> getDirection;
    private Coroutine positionReachedRoutine;
    private bool isIdle;
    private float idleTimer;

    public event UnityAction OnStart;
    public event UnityAction OnEnd;
    public event UnityAction<Vector3> OnUpdate;

    public float Speed => speed;

    public bool IsAtDestination => state switch
    {
        MoveState.POSITION => Vector3.Distance(transform.position, getTargetPosition()) < 0.1f,
        MoveState.DIRECTION => false,
        _ => false,
    };

    #region Public Methods
    public void SetTarget(Transform target)
    {
        state = MoveState.POSITION;
        isIdle = false;
        if (positionReachedRoutine != null) StopCoroutine(positionReachedRoutine);

        getTargetPosition = () =>
        {
            var direction = target.position - transform.position;
            return target.position - direction.normalized * 2f;
        };

        getDirection = () => getTargetPosition() - transform.position;
    }

    public void SetDirection(Vector3 direction)
    {
        state = MoveState.DIRECTION;
        isIdle = false;

        getDirection = () => direction;
    }

    public void SetPosition(Vector3 position, UnityAction onComplete = null)
    {
        state = MoveState.POSITION;

        if (animator) animator.SetBool("isMoving", true);

        getTargetPosition = () => position;
        getDirection = () => position - transform.position;

        OnStart?.Invoke();

        if (positionReachedRoutine != null) StopCoroutine(positionReachedRoutine);
        positionReachedRoutine = StartCoroutine(WaitUntilDestinationReached(onComplete));
    }

    public void StartRandomMovement()
    {
        if (startIdle) StartIdle();
        else StopIdle();
    }

    public void Stop()
    {
        isIdle = false;
        SetPosition(transform.position);
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

    private void Awake()
    {
        Stop();
    }

    private void Update()
    {
        if (isIdle) UpdateIdle();
        else UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (IsAtDestination) return;

        var direction = getDirection().normalized;

        transform.position += speed * Time.deltaTime * direction;
        if (direction != Vector3.zero) transform.forward = direction;

        OnUpdate?.Invoke(direction);
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
        SetPosition(positionAnchor.Position, StartIdle);
    }

    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > maxIdleDuration) StopIdle();
    }
}
