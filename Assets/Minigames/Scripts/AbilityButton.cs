using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected CooldownHandler cooldownHandler;
    [SerializeField] private float maxRange = 3f;

    public bool IsOnCooldown => cooldownHandler.IsOnCooldown;
    public float MaxRange => maxRange;

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
            UpdateIndicator();
        }
    }

    private void OnDragUpdated(Vector3 direction)
    {
        this.direction = direction;
        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        var clampedDirection = Vector3.ClampMagnitude(direction * 0.01f, ability.MaxRange);
        var startPosition = playerReference.Movement.transform.position;
        var targetPosition = startPosition + clampedDirection;
        abilityCastIndicator.UpdateIndicator(playerReference.Movement.transform.position, targetPosition);
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
