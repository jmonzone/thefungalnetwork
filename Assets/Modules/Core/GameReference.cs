using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class GameReference : ScriptableObject
{
    [SerializeField] private MultiplayerReference multiplayer;

    [SerializeField] private GamePlayer clientPlayer;
    [SerializeField] private List<GamePlayer> players;

    public bool isComplete;
    public GameMode gameMode;

    public GamePlayer ClientPlayer => clientPlayer;
    public List<GamePlayer> Players => players;

    public event UnityAction OnClientPlayerAdded;
    public event UnityAction OnAllPlayersAdded;

    public event UnityAction OnGameStart;
    public event UnityAction OnGameComplete;

    public event UnityAction OnScoreUpdated;

    public event UnityAction<int, int> OnKill;
    public event UnityAction<GamePlayer> OnSelfDestruct;

    public void Initialize()
    {
        clientPlayer = null;
        players = new List<GamePlayer>();
        isComplete = false;
    }

    // todo: pass LobbyPlayer object
    // todo: add clientID field to LobbyPlayer
    public void AddPlayer(ulong clientId, FixedString64Bytes playerName, int playerIndex, NetworkFungal networkFungal)
    {
        // Check if a player with the same index already exists
        if (Players.Any(player => player.Fungal == networkFungal))
        {
            Debug.Log($"Player with fungal {networkFungal.name} already exists. Skipping registration.");
            return; // Skip if player already exists
        }

        Enum.TryParse(multiplayer.GetJoinedLobbyData("GameMode"), out gameMode);

        Debug.Log($"addplayer {clientId} {playerName} {playerIndex} {networkFungal.name}");
        var addedPlayer = new GamePlayer(clientId, playerIndex, playerName.ToString(), networkFungal);
        Players.Add(addedPlayer);

        // Sort the list based on the player index
        Players.Sort((player1, player2) => player1.Index.CompareTo(player2.Index));

        if (networkFungal.IsOwner && !networkFungal.IsAI)
        {
            clientPlayer = addedPlayer;

            Debug.Log($"OnClientPlayerAdded");
            OnClientPlayerAdded?.Invoke();
        }

        if (multiplayer.LobbyPlayers.Count == Players.Count)
        {
            OnAllPlayersAdded?.Invoke();
        }

        if (gameMode== GameMode.PARTY)
        {
            addedPlayer.Fungal.OnScoreUpdated += () => OnScoreUpdated?.Invoke();
        }
        else
        {
            addedPlayer.Fungal.OnLivesChanged += Fungal_OnDeath;
        }

        addedPlayer.Fungal.OnDeath += selfDestruct =>
        {
            if (selfDestruct) OnSelfDestruct?.Invoke(addedPlayer);
        };

        addedPlayer.Fungal.OnKill += (x, y) =>
        {
            if (isComplete) return;
            OnKill?.Invoke(x, y);
        };
    }

    private void Fungal_OnDeath()
    {
        if (isComplete) return;


        // Check how many players are still alive
        var alivePlayers = Players.Where(player => player.Lives > 0).ToList();

        if (Players.Count == 1 && alivePlayers.Count == 0)
        {
            var winner = Players[0];
            winner.IsWinner = true;
            EndGame();
        }
        else if (Players.Count > 1 && alivePlayers.Count == 1)
        {
            var winner = alivePlayers[0];
            winner.IsWinner = true;
            EndGame();
        }
    }

    public void StartGame()
    {
        OnGameStart?.Invoke();
    }

    public void EndGame()
    {
        isComplete = true;
        clientPlayer.Fungal.Movement.Stop();

        var highestScore = Players.Max(p => p.Score);

        foreach (var player in Players)
        {
            player.IsWinner = player.Score == highestScore;
        }

        OnGameComplete?.Invoke();
    }
}
