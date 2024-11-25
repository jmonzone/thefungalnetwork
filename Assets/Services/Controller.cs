using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    public MovementController Movement { get; private set; }
    public Volume Volume { get; set; }

    public void Initialize(Volume volume)
    {
        Volume = volume;
    }

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
