using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class AutoConnect : NetworkBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Transform playerSpawnAnchor;

    private void Awake()
    {
        arena.Initialize(playerSpawnAnchor.position);
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
                    if (lobbies.Count > 0)
                    {
                        await multiplayer.TryJoinLobbyById(lobbies[0].Id);
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

    [ClientRpc]
    public void NotifyClientsDisconnectClientRpc()
    {
        Debug.Log("AutoConnect NotifyClientsDisconnectClientRpc");
        multiplayer.DisconnectFromRelay();
    }

}
