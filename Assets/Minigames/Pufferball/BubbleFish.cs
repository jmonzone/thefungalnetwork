using Unity.Netcode;
using UnityEngine;

public class BubbleFish : NetworkBehaviour
{
    private Fish fish;
    private bool canHit = false;

    private void Awake()
    {
        fish = GetComponent<Fish>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += ThrowFish_OnThrowComplete;
    }

    private void ThrowFish_OnThrowComplete()
    {
        canHit = true;
    }

    private void Update()
    {
        if (IsOwner && canHit)
        {
            CheckPlayerHit();
        }
    }

    private void CheckPlayerHit()
    {
        // Detect all players in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider hit in hitColliders)
        {
            var fungal = hit.GetComponent<NetworkFungal>();
            if (fungal != null)
            {
                fungal.ModifySpeedServerRpc(0f, 1.5f);
                fungal.TakeDamageServerRpc(1f);
                fish.ReturnToRadialMovement();

                canHit = false;
            }
        }
    }
}
