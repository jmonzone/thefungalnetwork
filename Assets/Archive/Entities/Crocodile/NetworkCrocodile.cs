using Unity.Netcode;
using UnityEngine;

public class NetworkCrocodile : NetworkBehaviour
{
    private AbilityCast abilityCast;

    private void Awake()
    {
        abilityCast = GetComponent<AbilityCast>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        var ai = GetComponent<CrocodileAI>();
        ai.enabled = IsOwner;

        var charge = GetComponent<CrocodileCharge>();
        charge.enabled = IsOwner;

        if (IsOwner)
        {
            abilityCast.OnPrepare += OnAbilityStart;
            abilityCast.OnCastStart += OnAbilityCast;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            abilityCast.OnPrepare -= OnAbilityStart;
            abilityCast.OnCastStart -= OnAbilityCast;
        }
    }

    private void OnAbilityStart()
    {
        OnAbilityStartClientRpc(NetworkManager.Singleton.LocalClientId, abilityCast.Direction);
    }

    [ClientRpc]
    private void OnAbilityStartClientRpc(ulong clientId, Vector3 direction)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        abilityCast.SetDirection(direction);
        abilityCast.Prepare();
    }

    private void OnAbilityCast()
    {
        OnAbilityCastClientRpc(NetworkManager.Singleton.LocalClientId);
    }


    [ClientRpc]
    private void OnAbilityCastClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;

        Debug.Log("on ability Start client RPC");

        abilityCast.StartCast();
    }
}
