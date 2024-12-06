using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Transform spawnAnchor1;
    //[SerializeField] private Button exitButton;

    private List<string> firstNames = new List<string> { "Hollow", "Sharon", "Jesus", "Charmander", "Feni" };
    private List<string> lastNames = new List<string> { "Zozo", "Venga", "Lotus", "Atari", "Bagus" };

    private void Awake()
    {
        arena.Initialize(spawnAnchor1.position, spawnAnchor1.position);
    }

    private void Start()
    {
        var username = GenerateRandomName();
        multiplayerManager.SignIn(username, () => StartCoroutine(AutoJoinBogRoom()));
    }

    public string GenerateRandomName()
    {
        string firstName = firstNames[Random.Range(0, firstNames.Count)];
        string lastName = lastNames[Random.Range(0, lastNames.Count)];
        return $"{firstName} {lastName}";
    }

    private IEnumerator AutoJoinBogRoom()
    {
        yield return new WaitForSeconds(2f);

        multiplayerManager.ListLobbies(async lobbies =>
        {
            // Check if the player is already in a lobby
            var rejoined = await multiplayerManager.TryRejoinLobby();
            if (rejoined) return;

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
