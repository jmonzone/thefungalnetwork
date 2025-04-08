using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Sprite image;


    public string Id => id;
    public Color BackgroundColor => backgroundColor;
    public Sprite Image => image;

    [SerializeField] protected float radius = 1f;
    [SerializeField] private float castCooldown = 2f;

    private CooldownModel cooldownModel;
    protected NetworkFungal fungal;

    public bool IsAvailable { get; private set; } = true;
    public bool IsOnCooldown => cooldownModel.IsOnCooldown;
    public CooldownModel Cooldown => cooldownModel;
    public float Radius => radius;

    public abstract Vector3 DefaultTargetPosition { get; }

    // todo: set input type directional or trajectory
    public abstract bool UseTrajectory { get; }
    public abstract float Range { get; }

    public event UnityAction OnAvailabilityChanged;
    public event UnityAction OnCancel;


    public event UnityAction OnAbilityStart;
    public event UnityAction OnAbilityComplete;

    public virtual void PrepareAbility() { }
    public virtual void ChargeAbility() { }

    public virtual void Initialize(NetworkFungal fungal)
    {
        this.fungal = fungal;
        cooldownModel = new CooldownModel(castCooldown);
    }

    public virtual void CastAbility(Vector3 targetPosition)
    {
        OnAbilityStart?.Invoke();
    }

    protected void CompleteAbility()
    {
        OnAbilityComplete?.Invoke();
    }

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
