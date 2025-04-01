using System;
using UnityEngine;

public class FungalThrow : Ability
{
    [SerializeField] private PufferballReference pufferballReference;

    private FishPickup fishPickup;

    public event Action<Fish> OnFishChanged;

    public override bool UseTrajectory => fishPickup.Fish.UseTrajectory;
    public override float Range => fishPickup.Fish.ThrowFish.Range;

    private void Start()
    {
        if (pufferballReference.ClientPlayer != null)
        {
            PufferballReference_OnPlayerRegistered();
        }
        else
        {
            pufferballReference.OnClientPlayerAdded += PufferballReference_OnPlayerRegistered;
        }
    }

    private void PufferballReference_OnPlayerRegistered()
    {
        pufferballReference.OnClientPlayerAdded -= PufferballReference_OnPlayerRegistered;
        fishPickup = pufferballReference.ClientPlayer.Fungal.GetComponent<FishPickup>();
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
        fishPickup.Sling(targetPosition);
    }

    public override Vector3 DefaultTargetPosition
    {
        get
        {
            Vector3 origin = pufferballReference.ClientPlayer.Fungal.transform.position;
            Vector3 forwardTarget = origin + pufferballReference.ClientPlayer.Fungal.transform.forward * Range;
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
                if (fungal == pufferballReference.ClientPlayer.Fungal) continue;

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
