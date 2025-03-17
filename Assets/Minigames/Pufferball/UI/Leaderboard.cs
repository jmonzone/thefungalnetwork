using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private LeaderboardEntry leaderboardEntryPrefab;
    [SerializeField] private MultiplayerManager multiplayer;

    private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

    private void Awake()
    {
        GetComponentsInChildren(includeInactive: true, leaderboardEntries);
    }

    private void Start()
    {
        PufferballMinigame_OnScoreUpdated();
    }

    private void OnEnable()
    {
        pufferball.OnScoreUpdated += PufferballMinigame_OnScoreUpdated;
    }

    private void OnDisable()
    {
        pufferball.OnScoreUpdated -= PufferballMinigame_OnScoreUpdated;
    }

    private void PufferballMinigame_OnScoreUpdated()
    {
        Debug.Log("PufferballMinigame_OnScoreUpdated");

        var players = pufferball.Players;
        var clientPlayer = pufferball.Player;

        // Sort by score descending, client player first if tied
        var sortedPlayers = players
            .OrderByDescending(p => p.Score)
            .ThenByDescending(p => p == clientPlayer)
            .ToList();

        // Build leaderboard: top 3 plus client if missing
        var leaderboardPlayers = sortedPlayers.Take(3).ToList();

        if (!leaderboardPlayers.Contains(clientPlayer))
        {
            leaderboardPlayers.Add(clientPlayer);
        }

        int playerCount = leaderboardPlayers.Count;

        // Ensure we have enough UI entries
        while (leaderboardEntries.Count < playerCount)
        {
            // Instantiate a new entry and add it to the list
            LeaderboardEntry entry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            leaderboardEntries.Add(entry);
        }

        // Update and enable the leaderboard entries
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            var entry = leaderboardEntries[i];

            if (i < playerCount)
            {
                var player = leaderboardPlayers[i];

                // Assuming you have a way to get the player icon and name
                Sprite playerIcon = player.fungal.Data.ActionImage; // Example property

                var localPlayerIndex = player.index;
                var localPlayer = multiplayer.JoinedLobby.Players[localPlayerIndex];

                string playerName = localPlayer.Data.TryGetValue("PlayerName", out var playerNameData)
                    ? playerNameData.Value
                    : "Unknown Player";

                var playerPoints = player.Score;

                entry.SetEntry(playerIcon, playerName, playerPoints, player == clientPlayer);

                entry.gameObject.SetActive(true);
            }
            else
            {
                // Hide unused entries
                entry.gameObject.SetActive(false);
            }
        }
    }
}
