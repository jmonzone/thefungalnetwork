using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bubble : NetworkBehaviour
{
    [SerializeField] private GameObject render;

    private bool canHit = true;

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
                OnPlayerHitServerRpc();

                canHit = false;
            }
        }
    }

    [ServerRpc]
    private void OnPlayerHitServerRpc()
    {
        OnPlayerHitClientRpc();
    }

    [ClientRpc]
    private void OnPlayerHitClientRpc()
    {
        render.SetActive(false);
    }
}
