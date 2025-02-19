using UnityEngine;

public class FishingRodButton : MonoBehaviour
{
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private CooldownHandler cooldownHandler;
    [SerializeField] private AbilityCastIndicator abilityCastIndicator;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private float slingCooldown = 0.25f;

    private Vector3 direction;

    private void Awake()
    {
        directionalButton.OnDragStarted += DirectionalButton_OnDragStarted;
        directionalButton.OnDragUpdated += DirectionalButton_OnDragUpdated;
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
        pufferballReference.OnFishingRodUpdated += PufferballReference_OnFishingRodUpdated;
    }

    private void DirectionalButton_OnDragUpdated(Vector3 direction)
    {
        this.direction = direction;
    }

    private void Update()
    {
        if (directionalButton.CastStarted)
        {
            var clampedDirection = Vector3.ClampMagnitude(direction * 0.01f, 3f);
            abilityCastIndicator.UpdateIndicator(playerReference.Transform.position, clampedDirection);
        }
    }

    private void DirectionalButton_OnDragStarted()
    {
        abilityCastIndicator.ShowIndicator();
    }

    private void PufferballReference_OnFishingRodUpdated()
    {
        pufferballReference.FishingRod.OnPufferfishReleased += FishingRod_OnPufferfishReleased;
    }

    private void FishingRod_OnPufferfishReleased()
    {
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
    }

    private void DirectionalButton_OnDragCompleted(Vector3 direction)
    {
        if (cooldownHandler.IsOnCooldown) return;

        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        var pufferfish = pufferballReference.FishingRod.Pufferfish;
        if (pufferfish)
        {
            pufferballReference.FishingRod.Sling(direction);
            cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
        }
        else
        {
            pufferballReference.FishingRod.Cast(direction, pufferfishCaught =>
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

        abilityCastIndicator.HideIndicator();
    }
}
