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
    private void OnPlayerDefeatedServerRpc(ulong fungalId)
    {
        OnPLayerDefeatedClientRpc(fungalId);
    }

    [ClientRpc]
    private void OnPLayerDefeatedClientRpc(ulong fungalId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fungalId, out var networkObject))
        {
            Debug.Log("OnPLayerDefeatedClientRpc");
            pufferball.UpdateScore(networkObject);
        }
    }
}
