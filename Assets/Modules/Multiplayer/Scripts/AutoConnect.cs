using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class AutoConnect : NetworkBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private bool addAIPlayer = false;
    [SerializeField] private GameMode gameMode;

    private async void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.LogLevel = LogLevel.Error;
        }

        if (multiplayer.JoinedLobby != null)
        {
            Debug.Log($"AutoConnect multiplayer.IsHost: {multiplayer.IsHost}");
            if (multiplayer.IsHost && bool.TryParse(multiplayer.GetJoinedLobbyData("UseAI"), out bool useAI))
            {
                Debug.Log($"AutoConnect multiplayer.useAI: {useAI}");

                if (useAI) await multiplayer.AddAIPlayer("fungal GPT");
            }

            multiplayer.StartHostClient();
        }
        else
        {
            StartCoroutine(AutoConnectRoutine());
        }
    }

    private IEnumerator AutoConnectRoutine()
    {
        yield return new WaitForSeconds(3f);

        multiplayer.SignIn(() =>
        {
            multiplayer.ListLobbies(async lobbies =>
            {
                if (lobbies.Count > 0)
                {
                    var value = await multiplayer.TryJoinLobbyById(lobbies[0].Id);
                    if (value)
                    {
                        var joinCode = multiplayer.JoinedLobby.Data["JoinCode"].Value;
                        multiplayer.JoinRelay(joinCode);
                    }
                }
                else
                {
                    await multiplayer.CreateRelayAndLobby(gameMode);
                    if (addAIPlayer) await multiplayer.AddAIPlayer("fungal GPT");
                    multiplayer.StartHostClient();
                }
            });
        });
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
