using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Throw")]
public class FungalThrow : Ability
{
    private FishPickup fishPickup;

    public event Action<Fish> OnFishChanged;

    public override bool UseTrajectory => fishPickup.Fish.UseTrajectory;
    public override float Range => fishPickup.Fish.ThrowFish.Range;

    public override void Initialize(NetworkFungal fungal)
    {
        base.Initialize(fungal);
        fishPickup = fungal.GetComponent<FishPickup>();
        fishPickup.OnFishChanged += FishPickup_OnFishChanged;
        fishPickup.OnFishReleased += FishPickup_OnFishReleased;

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

    public override Vector3 DefaultTargetPosition
    {
        get
        {
            Vector3 origin = fungal.transform.position;
            Vector3 forwardTarget = origin + fungal.transform.forward * Range;
            float searchRadius = Range;
            LayerMask targetLayer = ~0;

            Collider[] colliders = Physics.OverlapSphere(origin, searchRadius, targetLayer);
            NetworkFungal closestFungal = null;
            float closestDistance = Mathf.Infinity;

            foreach (var collider in colliders)
            {
                NetworkFungal fungal = collider.GetComponent<NetworkFungal>();
                if (fungal == null) continue;
                if (fungal.IsDead) continue;
                if (this.fungal == fungal) continue;

                float distance = Vector3.Distance(origin, fungal.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFungal = fungal;
                }
            }

            return closestFungal != null ? closestFungal.transform.position : forwardTarget;
        }
    }
}
