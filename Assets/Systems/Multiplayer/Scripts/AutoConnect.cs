using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
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
        multiplayerManager.SignIn(displayName.name, () => StartCoroutine(AutoJoinBogRoom()));
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
