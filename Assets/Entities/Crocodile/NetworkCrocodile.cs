using Unity.Netcode;
using UnityEngine;

public class NetworkCrocodile : NetworkBehaviour
{
    private CrocodileCharge attack;
    private AbilityCast abilityCast;

    private void Awake()
    {
        attack = GetComponent<CrocodileCharge>();
        attack.enabled = false;

        abilityCast = GetComponent<AbilityCast>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            attack.enabled = true;
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
        Debug.Log("on ability Start");
        OnAbilityStartClientRpc(NetworkManager.Singleton.LocalClientId, abilityCast.Direction);
    }

    [ClientRpc]
    private void OnAbilityStartClientRpc(ulong clientId, Vector3 direction)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        
            Debug.Log("on ability Start client RPC");

        //todo: remove second param
        abilityCast.SetDirection(direction);
        abilityCast.Prepare();
    }

    private void OnAbilityCast()
    {
        //todo: centralize logic with AbilityCastController
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
