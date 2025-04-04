using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class FishingPlayer : NetworkBehaviour
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private MultiplayerReference multiplayer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            //Debug.Log("FishingPlayer.OnNetworkSpawn");

            PlayerSpawner playerSpawner = FindObjectOfType<PlayerSpawner>();
            if (playerSpawner.IsSpawned)
            {
                AddPlayerToSpawner(playerSpawner);
            }
            else
            {
                // Subscribe to event when spawner becomes available
                playerSpawner.OnSpawnerReady += AddPlayerToSpawner;
            }
        }
    }

    private void AddPlayerToSpawner(PlayerSpawner playerSpawner)
    {
        playerSpawner.OnSpawnerReady -= AddPlayerToSpawner; // Unsubscribe to avoid duplicates

        string localPlayerId = AuthenticationService.Instance.PlayerId;

        var localPlayerIndex = multiplayer.LobbyPlayers.FindIndex(player => player.lobbyId == localPlayerId);
        var localPlayer = multiplayer.LobbyPlayers[localPlayerIndex];

        var rpcPlayer = new LobbyPlayerRPCParam(localPlayer);

        //Debug.Log(playerSpawner.NetworkObjectId);
        Debug.Log($"FishingPLayer.AddPlayerToSpawner {localPlayer.fungal} {rpcPlayer.fungal} ");

        playerSpawner.AddPlayerServerRpc(NetworkManager.Singleton.LocalClientId, localPlayerIndex, rpcPlayer);
    }
}
