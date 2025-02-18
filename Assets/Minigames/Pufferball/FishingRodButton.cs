using UnityEngine;

public class FishingRodButton : MonoBehaviour
{
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private CooldownHandler cooldownHandler;
    [SerializeField] private CooldownView cooldownView; // Reference to cooldown UI

    private void Awake()
    {
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
    }

    private void DirectionalButton_OnDragCompleted(Vector3 direction)
    {
        if (cooldownHandler.IsOnCooldown) return;

        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        pufferballReference.FishingRod.CastFishingRod(direction);
        pufferballReference.FishingRod.enabled = false;

        cooldownHandler.StartCooldown(); // Start logic cooldown
        cooldownView.StartCooldown(cooldownHandler.CooldownTimer); // Start UI cooldown
    }
}
