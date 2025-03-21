using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodButton : Ability
{
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private float slingCooldown = 0.25f;
    [SerializeField] private float minRange = 1f;
    [SerializeField] private float maxRange = 4f;
    [SerializeField] private float rangeIncreaseSpeed = 1.5f;

    [Header("UI")]
    [SerializeField] private Image abilityBackground;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image fishIcon;
    [SerializeField] private Color defaultBackgroundColor;
    [SerializeField] private TextMeshProUGUI abilityText;

    private FishPickup fishPickup;

    private void Start()
    {
        //Debug.Log($"awake pufferballReference.Player");
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

        //Debug.Log("PufferballReference_OnPlayerRegistered");
        fishPickup = pufferballReference.ClientPlayer.Fungal.GetComponent<FishPickup>();
        fishPickup.OnFishChanged += FishPickup_OnFishChanged;
        fishPickup.OnFishReleased += FishPickup_OnFishReleased;
        cooldownHandler.SetInteractable(false);
        ToggleAvailable(false);
    }

    private void FishPickup_OnFishReleased()
    {
        //cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
        CancelAbility();
    }

    private void FishPickup_OnFishChanged()
    {
        //Debug.Log("FishPickup_OnFishChanged");

        var fish = fishPickup.Fish;
        ToggleAvailable(fish);

        if (fish)
        {
            cooldownHandler.SetInteractable(true);

            abilityBackground.color = fish.BackgroundColor;
            abilityText.text = fish.AbilityName;
            fishIcon.sprite = fish.Icon;

            abilityIcon.gameObject.SetActive(false);
            fishIcon.gameObject.SetActive(true);

        }
        else
        {
            cooldownHandler.SetInteractable(false);

            abilityBackground.color = defaultBackgroundColor;
            abilityIcon.gameObject.SetActive(true);
            fishIcon.gameObject.SetActive(false);
            abilityText.text = "No Fish Available";
        }
    }

    public override void PrepareAbility()
    {
        base.PrepareAbility();
        if (fishPickup.Fish)
        {
            fishPickup.Fish.PrepareThrow();
        }

        //range = minRange;
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

            // Search for NetworkFungal in range
            float searchRadius = range; // or any radius you want
            LayerMask targetLayer = ~0; // You can specify a layer mask if needed

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

            // If found, target the closest fungal, otherwise return the original position
            if (closestFungal != null)
            {
                return closestFungal.transform.position;
            }
            else
            {
                return forwardTarget;
            }
        }
    }

}
