using UnityEngine;

public class FungalThrow : Ability
{
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private float slingCooldown = 0.25f;
    [SerializeField] private float minRange = 1f;
    [SerializeField] private float maxRange = 4f;
    [SerializeField] private float rangeIncreaseSpeed = 1.5f;

    private FishPickup fishPickup;

    // Events to notify the UI about state changes
    public event System.Action<Fish> OnFishChanged;

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
            Vector3 forwardTarget = origin + pufferballReference.ClientPlayer.Fungal.transform.forward * range;
            float searchRadius = range;
            LayerMask targetLayer = ~0;

            Collider[] colliders = Physics.OverlapSphere(origin, searchRadius, targetLayer);
            NetworkFungal closestFungal = null;
            float closestDistance = Mathf.Infinity;

            foreach (var collider in colliders)
            {
                NetworkFungal fungal = collider.GetComponent<NetworkFungal>();
                if (fungal != null && fungal != pufferballReference.ClientPlayer.Fungal)
                {
                    float distance = Vector3.Distance(origin, fungal.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestFungal = fungal;
                    }
                }
            }

            return closestFungal != null ? closestFungal.transform.position : forwardTarget;
        }
    }
}
