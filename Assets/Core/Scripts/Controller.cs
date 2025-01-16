using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    [SerializeField] private ViewReference inputView;
    [SerializeField] private MovementController movement;
    [SerializeField] private Mountable mount;

    public Attackable Attackable { get; private set; }
    public AbilityCast AbilityCast { get; private set; }
    public MovementController Movement => movement;
    public Mountable Mount => mount;

    public Volume Volume { get; private set; }

    public event UnityAction OnUpdate;
    public event UnityAction OnCastStart;

    //todo: unneeded and misleading
    public void Initialize(Volume volume)
    {
        Volume = volume;
    }

    public void SetMovement(MovementController movement)
    {
        if (Movement != null ) Movement.Stop();
        this.movement = movement;
        movement.Stop();

        if (AbilityCast) AbilityCast.OnCastStart -= OnCastStart;
        AbilityCast = movement.GetComponent<AbilityCast>();
        if (AbilityCast) AbilityCast.OnCastStart += OnCastStart;

        mount = movement.GetComponent<Mountable>();
        OnUpdate?.Invoke();
    }

    public void SetAnimation()
    {
        var animator = Movement?.GetComponent<MovementAnimations>()?.Animator;
        if (animator) animator.SetTrigger("attack");
    }
}
