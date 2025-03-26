using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float range = 3f;
    [SerializeField] protected float radius = 1f;
    [SerializeField] private float castCooldown = 2f;

    private CooldownModel cooldownModel;
    protected NetworkFungal networkFungal;

    public bool IsAvailable { get; private set; } = true;
    public bool IsOnCooldown => cooldownModel.IsOnCooldown;
    public CooldownModel Cooldown => cooldownModel;
    public float Range => range;
    public float Radius => radius;
    public abstract Vector3 DefaultTargetPosition { get; }

    public event UnityAction OnAvailabilityChanged;
    public event UnityAction OnCancel;

    public virtual void PrepareAbility() { }
    public virtual void ChargeAbility() { }
    public abstract void CastAbility(Vector3 direction);

    private void Awake()
    {
        networkFungal = GetComponent<NetworkFungal>();
        cooldownModel = new CooldownModel(castCooldown);
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
