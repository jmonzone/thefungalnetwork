using UnityEngine;

public class FishingRodButton : MonoBehaviour
{
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private CooldownHandler cooldownHandler;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private float slingCooldown = 0.25f;

    private void Awake()
    {
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
        pufferballReference.OnFishingRodUpdated += PufferballReference_OnFishingRodUpdated;
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
    }
}
