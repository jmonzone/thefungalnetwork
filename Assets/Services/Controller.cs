using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    [SerializeField] private ViewReference inputView;

    public Controllable Controllable { get; private set; }

    public Attackable Attackable { get; private set; }
    public MovementController Movement => Controllable?.Movement;
    public ProximityInteraction Interactions => Controllable?.Interactions;

    public Volume Volume { get; private set; }

    //todo: centralize Possession logic
    public Possessable Possessable { get; private set; }
    public bool IsPossessing { get; private set; }

    public event UnityAction OnUpdate;
    public event UnityAction OnPossessionStart;
    public event UnityAction OnReleaseStart;
    public event UnityAction OnDeath;

    public void Initialize(Volume volume)
    {
        Volume = volume;
        IsPossessing = false;
    }

    public void SetController(Controllable controller)
    {
        if (Attackable) Attackable.OnDeath -= OnDeath;
        if (Movement != null ) Movement.Stop();

        Controllable = controller;
        Attackable = controller.Movement.GetComponent<Attackable>();
        if (Attackable) Attackable.OnDeath += OnDeath;

        controller.Movement.Stop();
        OnUpdate?.Invoke();
    }

    public void InitalizePosessable(Possessable possessable)
    {
        Possessable = possessable;
    }

    public void StartPossession(Possessable possessable)
    {
        Movement.Stop();
        IsPossessing = true;
        Possessable = possessable;
        OnPossessionStart?.Invoke();
    }

    public void CompletePossession()
    {
        SetController(Possessable.GetComponent<Controllable>());
        IsPossessing = false;
        Possessable.OnPossess();
    }

    public void ReleasePossession()
    {
        IsPossessing = true;
        Movement.Stop();
        OnReleaseStart?.Invoke();
        Possessable = null;
    }

    public void CompleteRelease()
    {
        IsPossessing = false;
    }


    public void SetAnimation()
    {
        var animator = Movement?.GetComponent<MovementAnimations>()?.Animator;
        if (animator) animator.SetTrigger("attack");
    }
}
