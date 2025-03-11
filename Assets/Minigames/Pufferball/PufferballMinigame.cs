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
            pufferball.OnPlayerDefeated += OnPufferballMinigameServerRpc;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer) pufferball.OnPlayerDefeated -= OnPufferballMinigameServerRpc;
    }

    [ServerRpc]
    private void OnPufferballMinigameServerRpc(ulong fungalId)
    {
        OnPufferballMinigameClientRpc(fungalId);
    }

    [ClientRpc]
    private void OnPufferballMinigameClientRpc(ulong fungalId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fungalId, out var networkObject))
        {
            Debug.Log("OnPufferballMinigameClientRpc");
            pufferball.UpdateScore(networkObject);
        }
    }
}
