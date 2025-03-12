using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected CooldownHandler cooldownHandler;
    [SerializeField] protected float range = 3f;
    [SerializeField] protected float radius = 1f;

    public bool IsOnCooldown => cooldownHandler.IsOnCooldown;
    public float Range => range;
    public float Radius => radius;

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
    [SerializeField] private bool useTrajectory = false;
    [SerializeField] private bool useTargetIndicator = false;

    private void Awake()
    {
        directionalButton.OnDragStarted += OnDragStarted;
        directionalButton.OnDragUpdated += OnDragUpdated;
        directionalButton.OnDragCompleted += OnDragCompleted;
        directionalButton.OnDragCanceled += OnDragCanceled;
        ability.OnCancel += OnDragCanceled;
    }

    private void OnDragStarted()
    {
        if (ability.IsOnCooldown) return;
        ability.PrepareAbility();
        abilityCastIndicator.ShowIndicator(useTrajectory);
        abilityCastIndicator.ShowTargetIndicator(useTargetIndicator);
    }

    private void OnDragUpdated(Vector3 direction)
    {
        ability.ChargeAbility();

        var clampedDirection = Vector3.ClampMagnitude(direction, ability.Range);
        var startPosition = playerReference.Movement.transform.position;
        var targetPosition = startPosition + clampedDirection;
        abilityCastIndicator.UpdateIndicator(playerReference.Movement.transform.position, targetPosition, ability.Range);

        abilityCastIndicator.SetTargetIndicatorRadius(ability.Radius);
    }

    private void OnDragCompleted(Vector3 direction)
    {
        if (ability.IsOnCooldown) return;

        var clampedDirection = Vector3.ClampMagnitude(direction, ability.Range);
        var startPosition = playerReference.Movement.transform.position;
        var targetPosition = startPosition + clampedDirection;

        targetPosition.y = 0; // Keep it in the XZ plane

        ability.CastAbility(targetPosition);

        abilityCastIndicator.HideIndicator();
    }

    private void OnDragCanceled()
    {
        abilityCastIndicator.HideIndicator();
    }
}
