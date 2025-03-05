using UnityEngine;

public class MovementAnimations : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 1;

    private Movement movement;
    private Animator animator;

    public bool IsMoving { get; private set; }
    public float AnimationSpeed => animationSpeed;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if (!animator) return;

        IsMoving = movement.Type != Movement.MovementType.IDLE;

        // Set parameters locally
        animator.SetBool("isMoving", IsMoving);

        if (IsMoving)
        {
            float curvedSpeed = 1f - (1f / (1f + movement.CalculatedSpeed * 0.2f));
            animator.speed = animationSpeed * Mathf.Lerp(1f, 2f, curvedSpeed);
        }
        else
        {
            animator.speed = animationSpeed; // Default idle speed
        }
    }

    public void PlayHitAnimation()
    {
        animator.Play("Hit");
    }

    public void PlayDeathAnimation()
    {
        animator.Play("Death");
    }

    public void PlaySpawnAnimation()
    {
        animator.Play("Clicked");
    }
}
