using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pufferfish : MonoBehaviour
{
    private Transform targetTransform; // The current transform it follows (Fishing Rod or Player)
    private bool isCaught = false;     // Is the pufferfish currently caught?

    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float carryOffset = 1f; // Offset when being carried by the player

    private void Update()
    {
        if (targetTransform != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = targetTransform.position;

        // If being carried, apply an offset (to avoid clipping into player)
        if (!isCaught)
        {
            targetPosition += Vector3.up * carryOffset;
        }

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }

    // Called when the Fishing Rod catches the Pufferfish
    public void Catch(Transform fishingRod)
    {
        isCaught = true;
        targetTransform = fishingRod;
    }

    // Called when the Fishing Rod is disabled, making the Pufferfish follow the player
    public void PickUp(Transform player)
    {
        isCaught = false;
        targetTransform = player;
    }
}
