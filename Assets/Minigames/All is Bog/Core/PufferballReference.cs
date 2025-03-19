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
    public float Score;

    public bool IsWinner => Score > 1000f;

    public event UnityAction OnScoreUpdated;

    public Player(ulong clientId, NetworkFungal fungal, bool isAI = false)
    {
        ClientId = clientId;
        Index = fungal.Index;
        Fungal = fungal;
        Score = fungal.Score.Value;

        fungal.Score.OnValueChanged += (previousValue, newValue) =>
        {
            Score = newValue;
            OnScoreUpdated?.Invoke();
        };

        IsAI = isAI;
    }
}

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    [SerializeField] private Player clientPlayer;
    [SerializeField] private List<Player> players;

    private bool isComplete;

    public Player ClientPlayer => clientPlayer;
    public List<Player> Players => players;

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnClientPlayerAdded;
    public event UnityAction<Player> OnPlayerAdded;

    public void Initialize()
    {
        clientPlayer = null;
        players = new List<Player>();
        isComplete = false;
    }

    public void AddPlayer(ulong clientId, NetworkFungal networkFungal, bool isAi)
    {
        // Check if a player with the same index already exists
        if (Players.Any(player => player.Fungal == networkFungal))
        {
            Debug.Log($"Player with fungal {networkFungal.name} already exists. Skipping registration.");
            return; // Skip if player already exists
        }

        Debug.Log($"AddPlayer {clientId} {networkFungal.Index}");
        var addedPlayer = new Player(clientId, networkFungal, isAi);
        Players.Add(addedPlayer);

        // Sort the list based on the player index
        Players.Sort((player1, player2) => player1.Index.CompareTo(player2.Index));

        if (networkFungal.IsOwner)
        {
            clientPlayer = addedPlayer;
            OnClientPlayerAdded?.Invoke();
        }

        OnPlayerAdded?.Invoke(addedPlayer);

        addedPlayer.OnScoreUpdated += AddedPlayer_OnScoreUpdated;
    }

    private void AddedPlayer_OnScoreUpdated()
    {
        OnScoreUpdated?.Invoke();

        if (isComplete) return;
        foreach (var player in Players)
        {
            if (player.IsWinner)
            {
                isComplete = true;
                OnGameComplete?.Invoke();
                return;
            }
        }
    }
}
