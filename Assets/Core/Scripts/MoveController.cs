using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoveController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private MoveType type;

    public enum MoveType
    {
        STOP,
        TARGET,
        POSITION,
        DIRECTION,
    }

    public bool IsMovingToPosition { get; private set; }

    private Transform target;
    private Vector3 position;
    private Vector3 direction;
    private Coroutine positionReachedRoutine;

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
                MoveToPosition(target.position, 2f);
                break;
            case MoveType.POSITION:
                MoveToPosition(position, 0.1f);
                break;
        }
    }

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

    public void SetPosition(Vector3 position, UnityAction onComplete)
    {
        this.position = position;
        type = MoveType.POSITION;

        IsMovingToPosition = true;

        OnStart?.Invoke();

        if (positionReachedRoutine != null) StopCoroutine(positionReachedRoutine);
        positionReachedRoutine = StartCoroutine(WaitUntilDestinationReached(position, onComplete));
    }

    private IEnumerator WaitUntilDestinationReached(Vector3 position, UnityAction onComplete)
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, position) < 0.1f);
        onComplete?.Invoke();
        OnEnd?.Invoke();

        IsMovingToPosition = false;
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

    public void Stop()
    {
        type = MoveType.STOP;
    }

    private void MoveToPosition(Vector3 targetPosition, float distanceThreshold)
    {
        if (Vector3.Distance(transform.position, targetPosition) > distanceThreshold)
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
