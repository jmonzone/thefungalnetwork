using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bubble : NetworkBehaviour
{
    //[SerializeField] private Gam
    private void Update()
    {
        if (IsOwner)
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
                gameObject.SetActive(false);
            }
        }
    }
}
