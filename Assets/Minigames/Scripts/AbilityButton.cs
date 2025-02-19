using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected CooldownHandler cooldownHandler;
    [SerializeField] protected float range = 3f;

    public bool IsOnCooldown => cooldownHandler.IsOnCooldown;
    public float Range => range;

    public event UnityAction OnCancel;

    public virtual void PrepareAbility() { }
    public virtual void ChargeAbility() { }
    public abstract void CastAbility(Vector3 direction);

    protected void CancelAbility()
    {
        OnCancel?.Invoke();
    }
}

public class AbilityButton : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private AbilityCastIndicator abilityCastIndicator;
    [SerializeField] private Ability ability;

    private void Awake()
    {
        directionalButton.OnDragStarted += OnDragStarted;
        directionalButton.OnDragUpdated += OnDragUpdated;
        directionalButton.OnDragCompleted += OnDragCompleted;
        ability.OnCancel += Ability_OnCancel;
    }

    private void OnDragStarted()
    {
        if (ability.IsOnCooldown) return;
        ability.PrepareAbility();
        abilityCastIndicator.ShowIndicator();
    }

    private void OnDragUpdated(Vector3 direction)
    {
        ability.ChargeAbility();

        var clampedDirection = Vector3.ClampMagnitude(direction * 0.01f, ability.Range);
        var startPosition = playerReference.Movement.transform.position;
        var targetPosition = startPosition + clampedDirection;
        abilityCastIndicator.UpdateIndicator(playerReference.Movement.transform.position, targetPosition, ability.Range);
    }

    private void OnDragCompleted(Vector3 direction)
    {
        if (ability.IsOnCooldown) return;
        ability.CastAbility(direction);  // Cast the assigned ability
        abilityCastIndicator.HideIndicator();
    }

    private void Ability_OnCancel()
    {
        abilityCastIndicator.HideIndicator();
    }
}
