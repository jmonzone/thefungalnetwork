using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected CooldownHandler cooldownHandler;

    public bool IsOnCooldown => cooldownHandler.IsOnCooldown;

    public event UnityAction OnCancel;

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

    private Vector3 direction;

    private void Awake()
    {
        directionalButton.OnDragStarted += OnDragStarted;
        directionalButton.OnDragUpdated += OnDragUpdated;
        directionalButton.OnDragCompleted += OnDragCompleted;
        ability.OnCancel += Ability_OnCancel;
    }

    private void Ability_OnCancel()
    {
        abilityCastIndicator.HideIndicator();
    }

    private void Update()
    {
        if (directionalButton.CastStarted)
        {
            var clampedDirection = Vector3.ClampMagnitude(direction * 0.01f, 2f);
            abilityCastIndicator.UpdateIndicator(playerReference.Movement.transform.position, clampedDirection);
        }
    }

    private void OnDragUpdated(Vector3 direction)
    {
        this.direction = direction;
        abilityCastIndicator.UpdateIndicator(transform.position, direction);
    }

    private void OnDragStarted()
    {
        if (ability.IsOnCooldown) return;
        abilityCastIndicator.ShowIndicator();
    }

    private void OnDragCompleted(Vector3 direction)
    {
        if (ability.IsOnCooldown) return;
        ability.CastAbility(direction);  // Cast the assigned ability
        abilityCastIndicator.HideIndicator();
    }
}
