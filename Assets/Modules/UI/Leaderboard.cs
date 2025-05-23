using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private LeaderboardEntry leaderboardEntryPrefab;
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private bool showOnElimination;
    [SerializeField] private bool showAllPlayers;

    private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

    private void Awake()
    {
        GetComponentsInChildren(includeInactive: true, leaderboardEntries);
    }

    private void Start()
    {
        if (multiplayer.GameMode != GameMode.PARTY) gameObject.SetActive(showOnElimination);
        UpdateLeaderboard();
    }

    private void OnEnable()
    {
        game.OnScoreUpdated += UpdateLeaderboard;
        game.OnGameComplete += Pufferball_OnGameComplete;
        game.OnKill += Pufferball_OnKill;
        game.OnSelfDestruct += Pufferball_OnSelfDestruct;
    }

    private void OnDisable()
    {
        game.OnScoreUpdated -= UpdateLeaderboard;
        game.OnGameComplete -= Pufferball_OnGameComplete;
        game.OnKill -= Pufferball_OnKill;
        game.OnSelfDestruct -= Pufferball_OnSelfDestruct;
    }

    private void Pufferball_OnKill(int arg0, int arg1)
    {
        UpdateLeaderboard();
    }

    private void Pufferball_OnSelfDestruct(GamePlayer arg0)
    {
        UpdateLeaderboard();
    }

    private void Pufferball_OnGameComplete()
    {
        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        //Debug.Log($"updating leaderboard {multiplayer.GameMode }");

        if (multiplayer.GameMode == GameMode.PARTY)
        {
            UpdatePartyLeaderboard();
        }
        else
        {
            UpdateEliminationLeaderboard();
        }
    }

    private void UpdateEliminationLeaderboard()
    {
        var players = game.Players;
        var clientPlayer = game.ClientPlayer;

        // Sort by score descending, client player first if tied
        var sortedPlayers = players
            .OrderByDescending(p => p.Index)
            .ToList();

        int playerCount = sortedPlayers.Count;

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
                var player = sortedPlayers[i];

                // Assuming you have a way to get the player icon and name
                Sprite playerIcon = player.Fungal.Data.ActionImage; // Example property
                var playerPoints = $"{player.Fungal.Kills} BD | {player.Fungal.SelfDestructs} SU";

                entry.SetEliminationEntry(
                    playerIcon,
                    player.DisplayName,
                    playerPoints,
                    showOnElimination && player.IsWinner
                );

                entry.gameObject.SetActive(true);
            }
            else
            {
                // Hide unused entries
                entry.gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePartyLeaderboard()
    {
        //Debug.Log("UpdatePartyLeaderboard");

        var players = game.Players;
        var clientPlayer = game.ClientPlayer;

        // Sort by score descending, client player first if tied
        var sortedPlayers = players
            .OrderByDescending(p => p.Score)
            .ThenByDescending(p => p == clientPlayer)
            .ToList();

        var topPlayer = sortedPlayers[0];
        var topScore = topPlayer.Score;

        // Check if tied: more than 1 player with the same top score
        bool isTiedAtTop = sortedPlayers.Count > 1 && sortedPlayers[1].Score == topScore;
        isTiedAtTop = false;

        // Build leaderboard: top 3 plus client if missing
        var leaderboardPlayers = showAllPlayers ? sortedPlayers : sortedPlayers.Take(3).ToList();

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
                Sprite playerIcon = player.Fungal.Data.ActionImage; // Example property

                var playerPoints = player.Score.ToString();
                //Debug.Log("UpdatePartyLeaderboard " + playerPoints);

                // Determine if THIS player is the top player
                bool isTopPlayer = player == topPlayer;

                entry.SetPartyEntry(
                    playerIcon,
                    player.DisplayName,
                    playerPoints,
                    player == clientPlayer,
                    isTopPlayer && player.Score > 0,
                    isTiedAtTop && player.Score > 0
                );

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
