using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Throw")]
public class FungalThrow : DirectionalAbility
{
    private FishPickup fishPickup;

    public event Action<FishController> OnFishChanged;

    public override bool UseTrajectory => fishPickup.Fish.UseTrajectory;
    public override float Range => fishPickup.Fish?.ThrowFish.Range ?? 0;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
        fishPickup = fungal.GetComponent<FishPickup>();

        if (fishPickup)
        {
            fishPickup.OnFishChanged += FishPickup_OnFishChanged;
            fishPickup.OnFishReleased += FishPickup_OnFishReleased;
        }
        else
        {
            Debug.LogWarning($"Missing FishPickup component");
        }

        ToggleAvailable(false);
    }

    private void FishPickup_OnFishReleased()
    {
        CancelAbility();
    }

    private void FishPickup_OnFishChanged()
    {
        var fish = fishPickup.Fish;
        OnFishChanged?.Invoke(fish);
        ToggleAvailable(fish);
    }

    public override void PrepareAbility()
    {
        base.PrepareAbility();
        if (fishPickup.Fish)
        {
            fishPickup.Fish.PrepareThrow();
        }
    }

    public override void ChargeAbility()
    {
        base.ChargeAbility();
        var fish = fishPickup.Fish;
        if (fish)
        {
            var throwFish = fish.GetComponent<ThrowFish>();
            radius = throwFish.Radius;
        }
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility(targetPosition);
        fishPickup.Sling(targetPosition);
    }
}
