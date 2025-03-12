using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodButton : Ability
{
    [SerializeField] private PlayerReference playerReference;
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
        pufferballReference.OnPlayerRegistered -= PufferballReference_OnPlayerRegistered;

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

        var fish = fishPickup.Fish;
        if (fish)
        {
            cooldownHandler.SetInteractable(true);

            abilityBackground.color = fish.BackgroundColor;
            abilityText.text = fish.AbilityName;
            fishIcon.sprite = fish.Icon;

            abilityIcon.gameObject.SetActive(false);
            fishIcon.gameObject.SetActive(true);

        }
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

        var fish = fishPickup.Fish;
        if (fish)
        {
            var pufferfish = fish.GetComponent<Pufferfish>();

            if (pufferfish)
            {
                radius = pufferfish.ExplosionRadius;
            }
        }
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        fishPickup.Sling(targetPosition);
        cooldownHandler.SetInteractable(false);
        abilityBackground.color = defaultBackgroundColor;
        abilityIcon.gameObject.SetActive(true);
        fishIcon.gameObject.SetActive(false);
        abilityText.text = "No Fish Available";
    }
}
