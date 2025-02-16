using System.Collections;
using UnityEngine;

public class FishingRodButton : MonoBehaviour
{
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private FishingRodProjectile fishingRod;

    private void Awake()
    {
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
    }

    private void DirectionalButton_OnDragCompleted(Vector3 direction)
    {
        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        fishingRod.CastFishingRod(direction);
    }
}
