using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected CooldownHandler cooldownHandler;
    [SerializeField] protected float range = 3f;
    [SerializeField] protected float radius = 1f;

    public bool IsAvailable { get; private set; } = true;
    public bool IsOnCooldown => cooldownHandler.IsOnCooldown;
    public float Range => range;
    public float Radius => radius;
    public abstract Vector3 TargetPosition { get; }

    public event UnityAction OnAvailabilityChanged;
    public event UnityAction OnCancel;

    public virtual void PrepareAbility() { }
    public virtual void ChargeAbility() { }
    public abstract void CastAbility(Vector3 direction);

    protected void CancelAbility()
    {
        OnCancel?.Invoke();
    }

    protected void ToggleAvailable(bool value)
    {
        IsAvailable = value;
        OnAvailabilityChanged?.Invoke();
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
        //Debug.Log("AbilityButton.Awake");
        directionalButton.OnClick += DirectionalButton_OnClick;
        directionalButton.OnDragStarted += OnDragStarted;
        directionalButton.OnDragUpdated += OnDragUpdated;
        directionalButton.OnDragCompleted += OnDragCompleted;
        directionalButton.OnDragCanceled += OnDragCanceled;
        ability.OnCancel += OnDragCanceled;
        ability.OnAvailabilityChanged += UpdateAbility;
    }

    private void DirectionalButton_OnClick()
    {
        ability.CastAbility(ability.TargetPosition);
    }

    private void UpdateAbility()
    {
        directionalButton.enabled = ability.IsAvailable;
    }

    private void OnEnable()
    {
        if (!playerReference.Fungal) return;
        playerReference.Fungal.OnDeath += Fungal_OnDeath;
        playerReference.Fungal.OnRespawnComplete += UpdateAbility;
    }

    private void Fungal_OnDeath()
    {
        abilityCastIndicator.HideIndicator();
        directionalButton.enabled = false;
    }

    private void OnDisable()
    {
        playerReference.Fungal.OnDeath -= Fungal_OnDeath;
        playerReference.Fungal.OnRespawnComplete -= UpdateAbility;
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
