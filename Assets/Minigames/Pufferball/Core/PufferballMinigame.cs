using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PufferballMinigame : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            pufferball.OnPlayerDefeated += OnPlayerDefeatedServerRpc;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsServer) pufferball.OnPlayerDefeated -= OnPlayerDefeatedServerRpc;
    }


    [ServerRpc]
    private void OnPlayerDefeatedServerRpc(ulong fungalId, int source)
    {
        OnPlayerDefeatedClientRpc(fungalId, source);
    }
    [ClientRpc]
    private void OnPlayerDefeatedClientRpc(ulong fungalId, int source)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fungalId, out var networkObject))
        {
            Debug.Log("OnPLayerDefeatedClientRpc");
            pufferball.OnPlayerDeath(networkObject, source);
        }
    }
}
