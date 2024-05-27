using System;
using UnityEngine;
using UnityEngine.Events;

public enum MovementType
{
    DIRECTIONAL,
    TARGETED,
    RANDOM,
    TRAJECTORY
}

public class MovementController : MonoBehaviour
{
    [SerializeField] private bool is2D = true;
    [SerializeField] private float distanceThreshold = 0.5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector3 direction;

    private bool useTargetPosition = false;
    private bool useBounds = false;
    private Bounds bounds;
    private float time;

    public MovementType inputType;
    public bool faceForward;
    public bool normalizeSpeed;
    [SerializeField] private bool persistDirection;
    public Func<Vector3> getTargetPosition;

    public event UnityAction OnDestinationReached;

    private void LateUpdate()
    {
        if (inputType == MovementType.TRAJECTORY)
        {
            time += Time.deltaTime;
            if (transform.position.y > 0) direction.y -= 10f * Mathf.Pow(time, 2);
            else
            {
                direction = Vector3.zero;
                var floorPosition = transform.position;
                floorPosition.y = 0;
                transform.position = floorPosition;
                OnDestinationReached?.Invoke();
            }

            transform.position += Time.deltaTime * direction;
        }
        else
        {
            if (useTargetPosition && getTargetPosition != null)
            {
                var targetPosition = getTargetPosition();

                if (Vector3.Distance(targetPosition, transform.position) > distanceThreshold)
                {
                    direction = targetPosition - transform.position;
                }
                else
                {
                    if (useBounds) UpdateTargetPositionInBounds();
                    OnDestinationReached?.Invoke();
                }
            }
            else if (direction.magnitude == 0) return;

            if (normalizeSpeed) direction.Normalize();
            transform.position += speed * Time.deltaTime * direction;

        }

        if (faceForward) Forward = Vector3.Lerp(Forward, direction, 5f * Time.deltaTime);
        if (!persistDirection) direction = Vector3.zero;
    }

    public Vector3 Forward
    {
        get => is2D ? transform.up : transform.forward;
        set
        {
            if (is2D) transform.up = value;
            else transform.forward = value;
        }
    }

    public void Launch(Vector3 direction)
    {
        SetType(MovementType.TRAJECTORY);
        this.direction = direction;
        time = 0;
    }

    public void SetDirection(Vector3 direction)
    {
        SetType(MovementType.DIRECTIONAL);
        this.direction = direction;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        SetType(MovementType.TARGETED);
        getTargetPosition = () => targetPosition;
    }

    public void SetTarget(Transform target)
    {
        SetType(MovementType.TARGETED);
        getTargetPosition = () => target.position;
    }

    public void SetBounds(Bounds bounds)
    {
        SetType(MovementType.RANDOM);
        this.bounds = bounds;
        UpdateTargetPositionInBounds();
    }
    private void SetType(MovementType type)
    {
        this.inputType = type;
        useBounds = type == MovementType.RANDOM;
        useTargetPosition = type switch
        {
            MovementType.TARGETED => true,
            MovementType.RANDOM => true,
            _ => false,
        };
    }

    private void UpdateTargetPositionInBounds()
    {
        var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        var z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        var randomPosition = new Vector3(x, 0, z);
        getTargetPosition = () => randomPosition;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float DistanceThreshold
    {
        get => distanceThreshold;
        set => distanceThreshold = value;
    }

    public bool PersistDirection
    {
        get => persistDirection;
        set => persistDirection = value;
    }
}