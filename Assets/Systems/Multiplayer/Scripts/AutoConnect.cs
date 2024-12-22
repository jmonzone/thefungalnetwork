using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AutoConnect : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private Transform playerSpawnAnchor;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private Button exitButton;
    [SerializeField] private List<Transform> spores = new List<Transform>();

    private void Awake()
    {
        arena.Initialize(playerSpawnAnchor.position, spores);
    }

    private void OnEnable()
    {
        multiplayer.OnDisconnectRequested += Multiplayer_OnDisconnectRequested;
    }

    private void OnDisable()
    {
        multiplayer.OnDisconnectRequested -= Multiplayer_OnDisconnectRequested;
    }

    private void Multiplayer_OnDisconnectRequested()
    {
        NotifyClientsDisconnectServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void NotifyClientsDisconnectServerRpc()
    {
        Debug.Log("AutoConnect NotifyClientsDisconnectServerRpc");
        NotifyClientsDisconnectClientRpc();
    }

    // RPC method to notify other clients about disconnection
    [ClientRpc]
    public void NotifyClientsDisconnectClientRpc()
    {
        Debug.Log("AutoConnect NotifyClientsDisconnectClientRpc");
        multiplayer.DisconnectFromRelay();
    }

    private IEnumerator Start()
    {
        if (multiplayer.JoinedLobby != null)
        {
            yield return CreateRelay();
        }
        else
        {
            yield return new WaitForSeconds(3f);

            multiplayer.SignIn(() =>
            {
                multiplayer.ListLobbies(async lobbies =>
                {
                    // Check if the player is already in a lobby
                    //var rejoined = await multiplayerManager.TryRejoinLobby();
                    //if (rejoined) return;

                    if (lobbies.Count > 0)
                    {
                        await multiplayer.JoinLobbyById(lobbies[0].Id);
                    }
                    else
                    {
                        await multiplayer.CreateRelayAndLobby();
                    }
                });
            });
        }
    }

    private async Task CreateRelay()
    {
        if (multiplayer.IsHost)
        {
            var joinCode = await multiplayer.CreateRelay();
            await multiplayer.AddRelayToLobby(joinCode);
        }
        else
        {
            if (multiplayer.JoinedLobby.Data.ContainsKey("JoinCode"))
            {
                var joinCode = multiplayer.JoinedLobby.Data["JoinCode"].Value;
                multiplayer.JoinRelay(joinCode);
            }
        }
    }
}
