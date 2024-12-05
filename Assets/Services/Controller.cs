using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    public Controllable Controllable { get; private set; }
    public MovementController Movement => Controllable?.Movement;
    public ProximityInteraction Interactions => Controllable?.Interactions;

    public Volume Volume { get; set; }

    public event UnityAction OnUpdate;

    public void Initialize(Volume volume)
    {
        Volume = volume;
    }

    public void SetController(Controllable controller)
    {
        if (Movement != null ) Movement.Stop();
        Controllable = controller;
        controller.Movement.Stop();
        OnUpdate?.Invoke();
    }

    public void SetAnimation()
    {
        var animator = Movement?.GetComponent<MovementAnimations>()?.Animator;
        if (animator) animator.SetTrigger("attack");
    }
}
