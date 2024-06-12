using System;
using UnityEngine;

[RequireComponent(typeof(MoveController))]
public class RandomMovement : MonoBehaviour
{
    [SerializeField] private bool moveImmediate;
    [SerializeField] private float minIdleDuration = 2f;
    [SerializeField] private float maxIdleDuration = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private PositionAnchor positionAnchor;

    private MoveController movement;

    private bool isMoving;
    private float idleTimer;

    private void Awake()
    {
        movement = GetComponent<MoveController>();
    }

    private void OnEnable()
    {
        if (moveImmediate && positionAnchor.IsInitialized) StartMoving();
        else StopMoving();
    }

    private void Update()
    {
        if (isMoving)
        {
            if (!movement.IsMovingToPosition) StopMoving();
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > maxIdleDuration) StartMoving();
        }
    }

    private void StartMoving()
    {
        if (animator) animator.SetBool("isMoving", true);
        movement.SetPosition(positionAnchor.Position);
        isMoving = true;
    }

    private void StopMoving()
    {
        idleTimer = UnityEngine.Random.Range(0f, maxIdleDuration - minIdleDuration);
        if (animator) animator.SetBool("isMoving", false);
        isMoving = false;
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void SetBounds(Collider collider)
    {
        positionAnchor.Bounds = collider;
    }
}
