using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class FishingPlayer : NetworkBehaviour
{
    [SerializeField] private PlayerReference player;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private MultiplayerManager multiplayer;

    private NetworkFungal networkFungal;

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

        var localPlayerIndex = multiplayer.JoinedLobby.Players.FindIndex(player => player.Id == localPlayerId);
        var localPlayer = multiplayer.JoinedLobby.Players[localPlayerIndex];

        var characterIndex = localPlayer.Data.TryGetValue("Fungal", out var fungalData)
                ? int.TryParse(fungalData?.Value, out var index) ? index : 0 : 0;

        //Debug.Log(playerSpawner.NetworkObjectId);
        playerSpawner.AddPlayer(NetworkManager.Singleton.LocalClientId, displayName.Value, localPlayerIndex, characterIndex);
    }

    public void AssignFungal(NetworkFungal fungal)
    {
        networkFungal = fungal;
        player.SetMovement(networkFungal);
    }

}
