using UnityEngine;
using UnityEngine.Events;

public class ThrowFish : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private bool respawnOnThrow = true;
    [SerializeField] private float throwSpeed = 2f;
    [SerializeField] private float throwDistance = 10f;

    private FishController fish;
    private Movement movement;

    public float Radius => radius;
    public float Range => throwDistance;

    public Vector3 TargetPosition { get; private set; }

    public event UnityAction<Vector3> OnThrowStart;
    public event UnityAction OnThrowComplete;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        movement = GetComponent<Movement>();
        enabled = false;
    }

    private void OnEnable()
    {
        movement.OnDestinationReached += Movement_OnDestinationReached;
    }

    private void OnDisable()
    {
        movement.OnDestinationReached -= Movement_OnDestinationReached;
    }

    // todo: behaviour for throwing should be defined by the fish class
    public void Throw(Vector3 targetPosition)
    {
        enabled = true;
        targetPosition.y = 0; // Keep on the ground
        movement.SetSpeed(throwSpeed);

        if (fish.UseTrajectory)
        {
            movement.SetTrajectoryMovement(targetPosition); // Move towards the target
        }
        else
        {
            // todo: behaviour for wind fish should be defined by wind fish class
            var direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            var initialPosition = transform.position + direction;
            initialPosition.y = 0;
            transform.position = initialPosition;
            movement.SetTargetPosition(initialPosition + direction * throwDistance);
        }

        TargetPosition = targetPosition;
        OnThrowStart?.Invoke(targetPosition);
    }

    private void Movement_OnDestinationReached()
    {
        Debug.Log($"ThrowFish Movement_OnDestinationReached {name}");
        enabled = false;
        if (respawnOnThrow) fish.Respawn();
        OnThrowComplete?.Invoke();
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
