using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private float autoPopTime = 3f; // Adjust the time as needed

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
        StartCoroutine(AutoPopTimer()); // Start auto-pop countdown
    }

    private IEnumerator AutoPopTimer()
    {
        yield return new WaitForSeconds(autoPopTime);
        PopServerRpc();
    }

    private void Update()
    {
        if (IsOwner && canHit)
        {
            CheckPlayerHit();
        }
    }

    private bool hitRequested;

    private void CheckPlayerHit()
    {
        if (hitRequested) return;

        // Detect all players in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider hit in hitColliders)
        {
            var fungal = hit.GetComponent<NetworkFungal>();
            if (fungal != null)
            {
                fungal.ModifySpeedServerRpc(0f, 1.5f);
                fungal.TakeDamageServerRpc(1f);
                hitRequested = true;

                PopServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PopServerRpc()
    {
        Debug.Log("bubble.PopServerRpc()");
        if (canHit) PopClientRpc();
    }

    [ClientRpc]
    private void PopClientRpc()
    {
        if (IsOwner && canHit)
        {
            fish.ReturnToRadialMovement();
            canHit = false;
            hitRequested = false;
        }
    }
}
