using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoveController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2f;

    public Vector3 Direction { get; private set; }

    public event UnityAction OnStart;
    public event UnityAction OnEnd;
    public event UnityAction OnUpdate;

    public void MoveToTarget(Transform target, UnityAction onComplete)
    {
        StartCoroutine(MoveRoutine(target, onComplete));
    }

    public void LookAtTarget(Transform target)
    {
        var direction = target.position - transform.position;
        direction.y = 0;
        transform.forward = direction;
    }

    private IEnumerator MoveRoutine(Transform target, UnityAction onComplete)
    {
        OnStart?.Invoke();

        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            var direction = target.position - transform.position;
            direction.y = 0;
            MoveInDirection(direction.normalized);
            yield return null;
        }

        transform.position = target.position;

        OnEnd?.Invoke();
        onComplete?.Invoke();
    }

    public void MoveInDirection(Vector3 direction)
    {
        Direction = direction;
        transform.position += playerSpeed * Time.deltaTime * direction;
        if (direction.magnitude > 0) transform.forward = direction;

        OnUpdate?.Invoke();
    }
}
