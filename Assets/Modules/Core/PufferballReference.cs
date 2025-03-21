using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Player
{
    public ulong ClientId;
    public bool IsAI;
    public int Index;
    public NetworkFungal Fungal;
    public float Score => Fungal.Score;
    public int Lives => Fungal.Lives;
    public bool IsWinner = false;

    public Player(ulong clientId, int playerIndex, NetworkFungal fungal, bool isAI = false)
    {
        ClientId = clientId;
        Index = playerIndex;
        Fungal = fungal;
        IsAI = isAI;
    }
}

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    [SerializeField] private Player clientPlayer;
    [SerializeField] private List<Player> players;
    [SerializeField] private MultiplayerManager multiplayer;

    public bool isComplete;

    public Player ClientPlayer => clientPlayer;
    public List<Player> Players => players;

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnClientPlayerAdded;
    public event UnityAction<Player> OnPlayerAdded;
    public event UnityAction OnAllPlayersAdded;

    public void Initialize()
    {
        clientPlayer = null;
        players = new List<Player>();
        isComplete = false;
    }

    public void AddPlayer(ulong clientId, int playerIndex, NetworkFungal networkFungal, bool isAi)
    {
        // Check if a player with the same index already exists
        if (Players.Any(player => player.Fungal == networkFungal))
        {
            Debug.Log($"Player with fungal {networkFungal.name} already exists. Skipping registration.");
            return; // Skip if player already exists
        }

        Debug.Log($"AddPlayer {clientId} {playerIndex}");
        var addedPlayer = new Player(clientId, playerIndex, networkFungal, isAi);
        Players.Add(addedPlayer);

        // Sort the list based on the player index
        Players.Sort((player1, player2) => player1.Index.CompareTo(player2.Index));

        if (networkFungal.IsOwner)
        {
            clientPlayer = addedPlayer;
            OnClientPlayerAdded?.Invoke();
        }

        OnPlayerAdded?.Invoke(addedPlayer);
        if (multiplayer.JoinedLobby.Players.Count == Players.Count)
        {
            OnAllPlayersAdded?.Invoke();
        }

        if (multiplayer.GetGameMode(multiplayer.JoinedLobby) == GameMode.PARTY)
        {
            addedPlayer.Fungal.OnScoreUpdated += Fungal_OnScoreUpdated;
        }
        else
        {
            addedPlayer.Fungal.OnDeath += Fungal_OnDeath;
        }
    }

    private void Fungal_OnDeath()
    {
        if (isComplete) return;

        // Check how many players are still alive
        var alivePlayers = Players.Where(player => player.Lives > 0).ToList();

        // If only one player is alive, they win
        if (alivePlayers.Count == 1)
        {
            var winner = alivePlayers[0];
            winner.IsWinner = true;

            isComplete = true;
            Debug.Log($"GameComplete. Winner: {winner.Index}"); // Optional: add player name or ID for clarity
            clientPlayer.Fungal.Movement.Stop();
            OnGameComplete?.Invoke();
        }
    }


    private void Fungal_OnScoreUpdated()
    {
        OnScoreUpdated?.Invoke();

        if (isComplete) return;
        foreach (var player in Players)
        {
            player.IsWinner = player.Score >= 1000f;

            if (player.IsWinner)
            {
                isComplete = true;
                Debug.Log("GameComplete");
                clientPlayer.Fungal.Movement.Stop();
                OnGameComplete?.Invoke();
                return;
            }
        }
    }
}
