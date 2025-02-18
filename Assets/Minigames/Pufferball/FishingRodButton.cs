using UnityEngine;

public class FishingRodButton : MonoBehaviour
{
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private PufferballReference pufferballReference;

    private void Awake()
    {
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
    }

    private void DirectionalButton_OnDragCompleted(Vector3 direction)
    {
        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        pufferballReference.FishingRod.CastFishingRod(direction);
    }
}
