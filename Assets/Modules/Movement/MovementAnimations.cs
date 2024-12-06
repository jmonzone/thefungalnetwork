using Unity.Netcode;
using UnityEngine;

public class MovementAnimations : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 1;

    public Animator Animator => animator;

    public bool IsMoving { get; private set; }
    public float DirectionMagnitude { get; private set; }
    public float AnimationSpeed => animationSpeed;

    private void Awake()
    {
        if (!animator) enabled = false;
        movementController = GetComponentInParent<MovementController>();
    }

    public void Update()
    {
        IsMoving = !movementController.IsAtDestination;
        DirectionMagnitude = movementController.Direction.magnitude;

        // Set parameters locally
        animator.SetBool("isMoving", IsMoving);
        animator.speed = animationSpeed;
        if (IsMoving) animator.speed *= DirectionMagnitude / 1.5f;
    }
}
