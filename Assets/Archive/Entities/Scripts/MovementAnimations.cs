using UnityEngine;

public class MovementAnimations : MonoBehaviour
{
    private float animationSpeed = 0.5f;

    private Movement movement;
    private Animator animator;

    public bool isMoving;
    public float AnimationSpeed => animationSpeed;

    private void Awake()
    {
        movement = GetComponentInParent<Movement>();
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (!animator) return;

        // Set parameters locally
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            float curvedSpeed = 1f - (1f / (1f + movement.CalculatedSpeed * 0.2f));
            animator.speed = animationSpeed * Mathf.Lerp(1f, 2f, curvedSpeed);
        }
        else
        {
            animator.speed = animationSpeed; // Default idle speed
        }
    }

    public void SetIsMoving(bool value)
    {
        //Debug.Log($"SetIsMoving {movement.name} {isMoving}");
        isMoving = value;
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
