using Unity.Netcode;
using UnityEngine;

public class NetworkCrocodile : NetworkBehaviour
{
    private CrocodileAttack attack;
    [SerializeField] private AbilityCast abilityCast;

    private void Awake()
    {
        attack = GetComponent<CrocodileAttack>();
        attack.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            attack.enabled = true;
            abilityCast.OnStart += OnAbilityStart;
            abilityCast.OnComplete += OnAbilityCast;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            abilityCast.OnStart -= OnAbilityStart;
            abilityCast.OnComplete -= OnAbilityCast;
        }
    }

    private void OnAbilityStart()
    {
        Debug.Log("on ability Start");
        var targetClientId = abilityCast.Target.GetComponent<NetworkObject>();
        SendAbilityInfoClientRpc(NetworkManager.Singleton.LocalClientId, targetClientId.NetworkObjectId);
    }


    [ClientRpc]
    private void SendAbilityInfoClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        
            Debug.Log("on ability Start client RPC");

        // Retrieve the spawned object on the client using the NetworkObjectId
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            // Apply the rotated direction to the projectile
            //todo: remove third param
            abilityCast.StartCast(transform, networkObject.transform, attackable => true);
        }
        else
        {
            Debug.LogError("Failed to find the spawned object on the client.");
        }
    }



    private void OnAbilityCast()
    {
        //todo: centralize logic with AbilityCastController
        //RequestAbilityCastServerRpc(NetworkManager.Singleton.LocalClientId, abilityCast.Direction);
    }
}
