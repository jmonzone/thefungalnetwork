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

    private void Awake()
    {
        pufferballReference.OnFishingRodUpdated += PufferballReference_OnFishingRodUpdated;
    }

    private void PufferballReference_OnFishingRodUpdated()
    {
        pufferballReference.FishingRod.OnPufferfishReleased += FishingRod_OnPufferfishReleased;
    }

    private void FishingRod_OnPufferfishReleased()
    {
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
        CancelAbility();
    }

    public override void PrepareAbility()
    {
        base.PrepareAbility();
        range = minRange;
    }

    public override void ChargeAbility()
    {
        base.ChargeAbility();
        range = Mathf.Clamp(range + Time.deltaTime * rangeIncreaseSpeed, minRange, maxRange);
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        var pufferfish = pufferballReference.FishingRod.Pufferfish;
        if (pufferfish)
        {
            pufferballReference.FishingRod.Sling(targetPosition);
            cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
        }
        else
        {
            pufferballReference.FishingRod.Cast(targetPosition, pufferfishCaught =>
            {
                if (pufferfishCaught)
                {
                    cooldownHandler.SetInteractable(true);
                }
                else
                {
                    cooldownHandler.StartCooldown(castCooldown);
                }
            });

            cooldownHandler.SetInteractable(false);
        }
    }
}
