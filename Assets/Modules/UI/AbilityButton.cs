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
    public abstract Vector3 DefaultTargetPosition { get; }

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

    private bool isDown = false;

    private void Awake()
    {
        //Debug.Log("AbilityButton.Awake");
        directionalButton.OnPointerUp += DirectionalButton_OnPointerUp; ;
        directionalButton.OnPointerDown += DirectionalButton_OnPointerDown;
        directionalButton.OnDragUpdated += OnDragUpdated;
        directionalButton.OnDragCompleted += OnDragCompleted;
        directionalButton.OnDragCanceled += OnDragCanceled;
        ability.OnCancel += OnDragCanceled;
        ability.OnAvailabilityChanged += UpdateAbility;
    }

    private Vector3 targetPosition;
    private void LateUpdate()
    {
        if (isDown)
        {
            ability.ChargeAbility();

            var targetPosition = directionalButton.DragStarted ? this.targetPosition : ability.DefaultTargetPosition;
            abilityCastIndicator.UpdateIndicator(playerReference.Movement.transform.position, targetPosition, ability.Range);
            abilityCastIndicator.SetTargetIndicatorRadius(ability.Radius);
        }
    }
    private void OnEnable()
    {
        if (!playerReference.Fungal) return;
        playerReference.Fungal.OnDeath += Fungal_OnDeath;
        playerReference.Fungal.OnRespawnComplete += UpdateAbility;
    }

    private void OnDisable()
    {
        if (!playerReference.Fungal) return;
        playerReference.Fungal.OnDeath -= Fungal_OnDeath;
        playerReference.Fungal.OnRespawnComplete -= UpdateAbility;
    }

    private void Fungal_OnDeath(bool killed)
    {
        abilityCastIndicator.HideIndicator();
        directionalButton.enabled = false;
    }

    private void UpdateAbility()
    {
        directionalButton.enabled = ability.IsAvailable;
    }

    private void DirectionalButton_OnPointerDown()
    {
        if (ability.IsOnCooldown) return;

        isDown = true;
        ability.PrepareAbility();
        abilityCastIndicator.ShowIndicator(useTrajectory);
        abilityCastIndicator.ShowTargetIndicator(useTargetIndicator);
    }

    private void DirectionalButton_OnPointerUp()
    {
        if (ability.IsOnCooldown) return;
        CastAbility(ability.DefaultTargetPosition);
    }

    private void CastAbility(Vector3 targetPosition)
    {
        isDown = false;
        ability.CastAbility(targetPosition);
        abilityCastIndicator.HideIndicator();
    }

    private void OnDragUpdated(Vector3 direction)
    {
        var clampedDirection = Vector3.ClampMagnitude(direction, ability.Range);
        var startPosition = playerReference.Movement.transform.position;
        targetPosition = startPosition + clampedDirection;
        targetPosition.y = 0; // Keep it in the XZ plane
    }

    private void OnDragCompleted(Vector3 direction)
    {
        if (ability.IsOnCooldown) return;
        CastAbility(targetPosition);
    }

    private void OnDragCanceled()
    {
        abilityCastIndicator.HideIndicator();
    }
}
