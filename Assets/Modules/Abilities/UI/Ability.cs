using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Sprite image;
    [SerializeField] private GameObject prefab;

    public string Id => id;
    public Color BackgroundColor => backgroundColor;
    public Sprite Image => image;
    public GameObject Prefab => prefab;

    [SerializeField] protected float radius = 1f;
    [SerializeField] private float castCooldown = 2f;

    [SerializeField] private CooldownModel cooldownModel;

    public FungalController Fungal { get; protected set; }

    public bool InUse { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public bool IsOnCooldown => cooldownModel.IsOnCooldown;
    public CooldownModel Cooldown => cooldownModel;
    public float Radius => radius;

    public event UnityAction OnAvailabilityChanged;
    public event UnityAction OnCancel;
    public event UnityAction OnAbilityStart;
    public event UnityAction OnAbilityComplete;

    public virtual void PrepareAbility() { }
    public virtual void ChargeAbility() { }

    public virtual void Initialize(FungalController fungal)
    {
        Fungal = fungal;
        cooldownModel = new CooldownModel(castCooldown);
    }

    public virtual void CastAbility()
    {
        OnAbilityStart?.Invoke();
        Fungal.StartCoroutine(Cooldown.StartCooldown());

        InUse = true;
    }

    protected void CompleteAbility()
    {
        InUse = false;
        OnAbilityComplete?.Invoke();
    }

    protected void CancelAbility()
    {
        InUse = false;
        OnCancel?.Invoke();
    }

    protected void ToggleAvailable(bool value)
    {
        IsAvailable = value;
        OnAvailabilityChanged?.Invoke();
    }
}
