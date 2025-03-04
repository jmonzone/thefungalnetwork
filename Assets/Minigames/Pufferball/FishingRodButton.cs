using UnityEngine;

public class FishingRodButton : Ability
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private float slingCooldown = 0.25f;
    [SerializeField] private float minRange = 1f;
    [SerializeField] private float maxRange = 4f;
    [SerializeField] private float rangeIncreaseSpeed = 1.5f;

    private FishPickup fishPickup;

    private void Awake()
    {
        Debug.Log($"awake pufferballReference.Player");
        if (pufferballReference.Player)
        {
            PufferballReference_OnPlayerRegistered();
        }
        else
        {
            pufferballReference.OnPlayerRegistered += PufferballReference_OnPlayerRegistered;
        }
    }

    private void PufferballReference_OnPlayerRegistered()
    {
        Debug.Log("PufferballReference_OnPlayerRegistered");
        fishPickup = pufferballReference.Player.GetComponent<FishPickup>();
        fishPickup.OnFishChanged += FishPickup_OnFishChanged;
        fishPickup.OnFishReleased += FishPickup_OnFishReleased;
        cooldownHandler.SetInteractable(false);
    }

    private void FishPickup_OnFishReleased()
    {
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
        CancelAbility();
    }

    private void FishPickup_OnFishChanged()
    {
        Debug.Log("FishPickup_OnFishChanged");

        if (fishPickup.Fish) cooldownHandler.SetInteractable(true);
        else cooldownHandler.SetInteractable(false);
    }

    public override void PrepareAbility()
    {
        base.PrepareAbility();
        if (fishPickup.Fish)
        {
            fishPickup.Fish.PrepareThrow();
        }

        range = minRange;
    }

    public override void ChargeAbility()
    {
        base.ChargeAbility();
        range = Mathf.Clamp(range + Time.deltaTime * rangeIncreaseSpeed, minRange, maxRange);
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        fishPickup.Sling(targetPosition);
        cooldownHandler.SetInteractable(false);
    }
}
