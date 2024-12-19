using Unity.Netcode;
using UnityEngine;

public class NetworkCrocodile : NetworkBehaviour
{
    private Crocodile attack;
    [SerializeField] private AbilityCastReference abilityCast;

    private void Awake()
    {
        attack = GetComponent<Crocodile>();
        attack.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            attack.enabled = true;
            abilityCast.OnStart += OnAbilityStart;
            abilityCast.OnCast += OnAbilityCast;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            abilityCast.OnStart -= OnAbilityStart;
            abilityCast.OnCast -= OnAbilityCast;
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
        abilityCast.StartCast(transform, direction, attackable => true);
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

        abilityCast.Cast();
    }
}
