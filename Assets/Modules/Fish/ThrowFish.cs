using UnityEngine;
using UnityEngine.Events;

public class ThrowFish : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private bool respawnOnThrow = true;
    [SerializeField] private float throwSpeed = 2f;

    private Fish fish;
    private Movement movement;

    public float Radius => radius;
    public Vector3 TargetPosition { get; private set; }

    public event UnityAction<Vector3> OnThrowStart;
    public event UnityAction OnThrowComplete;

    private void Awake()
    {
        fish = GetComponent<Fish>();
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


    public void Throw(Vector3 targetPosition)
    {
        enabled = true;
        targetPosition.y = 0; // Keep on the ground
        movement.SetSpeed(throwSpeed);
        movement.SetTrajectoryMovement(targetPosition); // Move towards the target
        TargetPosition = targetPosition;
        OnThrowStart?.Invoke(targetPosition);
    }

    private void Movement_OnDestinationReached()
    {
        enabled = false;
        if (respawnOnThrow) fish.ReturnToRadialMovement();
        OnThrowComplete?.Invoke();
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
