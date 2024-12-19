using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private Transform playerSpawnAnchor;
    [SerializeField] private Transform crocodileSpawnAnchor;
    [SerializeField] private DisplayName displayName;

    private void Awake()
    {
        arena.Initialize(playerSpawnAnchor.position, crocodileSpawnAnchor.position);
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
