using UnityEngine;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    public MovementController Movement { get; private set; }

    public void SetMovement(MovementController movement)
    {
        Movement = movement;
    }

    public void SetAnimation()
    {
        var animator = Movement.GetComponent<MovementAnimations>().Animator;
        animator.SetTrigger("attack");
    }
}
