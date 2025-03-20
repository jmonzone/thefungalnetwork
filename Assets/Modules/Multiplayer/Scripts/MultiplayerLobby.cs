using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class MultiplayerLobby : ScriptableObject
{
    [SerializeField] private Lobby joinedLobby;

    public Lobby JoinedLobby => joinedLobby;

    private float lobbyUpdateTimer;

    public event UnityAction OnPoll;

    public async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby == null) return;

        lobbyUpdateTimer -= Time.deltaTime;
        if (lobbyUpdateTimer < 0f)
        {
            lobbyUpdateTimer = 1.1f;
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            joinedLobby = lobby;
            OnPoll?.Invoke();
        }
    }

}
