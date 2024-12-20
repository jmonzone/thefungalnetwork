using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    [SerializeField] private ViewReference inputView;
    [SerializeField] private FungalData fungal;
    [SerializeField] private MovementController movement;

    public FungalData Fungal => fungal;

    public Attackable Attackable { get; private set; }
    public AbilityCast AbilityCast { get; private set; }
    public MovementController Movement => movement;

    public Volume Volume { get; private set; }

    //todo: centralize Possession logic
    public Possessable Possessable { get; private set; }
    public bool IsPossessing { get; private set; }

    public event UnityAction OnInitialize;
    public event UnityAction OnUpdate;
    public event UnityAction OnPossessionStart;
    public event UnityAction OnReleaseStart;
    public event UnityAction OnDeath;
    public event UnityAction OnIsPossessingChanged;
    //todo: unneeded and misleading
    public void Initialize(Volume volume)
    {
        Volume = volume;
        SetIsPossessing(false);
    }

    public void Initialize(MovementController movement)
    {
        SetMovement(movement);
        OnInitialize?.Invoke();
    }

    private void SetIsPossessing(bool value)
    {
        IsPossessing = value;
        OnIsPossessingChanged?.Invoke();
    }

    public void SetMovement(MovementController movement)
    {
        if (Attackable) Attackable.OnHealthDepleted -= OnDeath;
        if (Movement != null ) Movement.Stop();

        this.movement = movement;
        Attackable = movement.GetComponent<Attackable>();
        AbilityCast = movement.GetComponent<AbilityCast>();
        if (Attackable) Attackable.OnHealthDepleted += OnDeath;

        fungal = movement.GetComponent<FungalController>()?.Model.Data;

        movement.Stop();
        OnUpdate?.Invoke();
    }

    public void InitalizePosessable(Possessable possessable)
    {
        Possessable = possessable;
    }

    public void StartPossession(Possessable possessable)
    {
        if (Movement) Movement.Stop();
        SetIsPossessing(true);
        Possessable = possessable;
        OnPossessionStart?.Invoke();
    }

    public void CompletePossession()
    {
        SetMovement(Possessable.GetComponent<MovementController>());
        SetIsPossessing(false);
        Possessable.OnPossess();
    }

    public void ReleasePossession()
    {
        SetIsPossessing(true);
        Movement.Stop();
        OnReleaseStart?.Invoke();
        Possessable = null;
    }

    public void CompleteRelease()
    {
        SetIsPossessing(false);
    }


    public void SetAnimation()
    {
        var animator = Movement?.GetComponent<MovementAnimations>()?.Animator;
        if (animator) animator.SetTrigger("attack");
    }
}
