using System.Collections;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Transform playerSpawnAnchor;
    [SerializeField] private Transform crocodileSpawnAnchor;
    [SerializeField] private DisplayName displayName;

    private void Awake()
    {
        arena.Initialize(playerSpawnAnchor.position, crocodileSpawnAnchor.position);
    }

    private void Start()
    {
        if (MultiplayerManager.Instance.JoinedLobby != null)
        {
            MultiplayerManager.Instance.CreateRelay();
        }
        else
        {
            multiplayerManager.SignIn(displayName.Value, () => StartCoroutine(AutoJoinBogRoom()));
        }
    }

    private void Update()
    {
        if (!MultiplayerManager.Instance.JoinedRelay && MultiplayerManager.Instance.JoinedLobby.Data.ContainsKey("JoinCode"))
        {
            Debug.Log("joining");
            var joinCode = MultiplayerManager.Instance.JoinedLobby.Data["JoinCode"].Value;
            MultiplayerManager.Instance.JoinRelay(joinCode);
        }
    }

    private IEnumerator AutoJoinBogRoom()
    {
        yield return new WaitForSeconds(2f);

        multiplayerManager.ListLobbies(async lobbies =>
        {
            // Check if the player is already in a lobby
            //var rejoined = await multiplayerManager.TryRejoinLobby();
            //if (rejoined) return;

            if (lobbies.Count > 0)
            {
                await multiplayerManager.JoinLobbyById(lobbies[0].Id);
            }
            else
            {
                await multiplayerManager.CreateRelayAndLobby();
            }
        });
    }
}
