using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoveController : MonoBehaviour
{
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

    public bool IsAtDestination => getTargetPosition != null && Vector3.Distance(transform.position, getTargetPosition()) < 0.1f;

    #region Public Methods
    public void SetTarget(Transform target)
    {
        StopIdle();

        getTargetPosition = () =>
        {
            var direction = target.position - transform.position;
            return target.position - direction.normalized * 2f;
        };

        getDirection = () => getTargetPosition() - transform.position;
    }

    public void SetDirection(Vector3 direction)
    {
        StopIdle();

        getTargetPosition = null;
        getDirection = () => direction;
    }

    public void SetPosition(Vector3 position, UnityAction onComplete = null)
    {
        StopIdle();

        if (animator) animator.SetBool("isMoving", true);

        getTargetPosition = () => position;
        getDirection = () => position - transform.position;

        OnStart?.Invoke();

        positionReachedRoutine = StartCoroutine(WaitUntilDestinationReached(onComplete));
    }

    public void StartRadialMovement(Vector3 origin)
    {
        StopIdle();

        var angle = 0f;
        getTargetPosition = () =>
        {
            angle += Time.deltaTime;
            var x = Mathf.Cos(angle);
            var z = Mathf.Sin(angle);
            var direction = new Vector3(x, 0, z) * 5f;
            return origin + direction;
        };
        getDirection = () => getTargetPosition() - transform.position;
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
        isIdle = false;
        if (positionReachedRoutine != null) StopCoroutine(positionReachedRoutine);
    }

    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > maxIdleDuration) SetPosition(positionAnchor.Position, StartIdle);
    }
}
