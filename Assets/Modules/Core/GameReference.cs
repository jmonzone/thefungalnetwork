using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

// todo: centralize or distinguish LobbyPlayer and Player objects
[Serializable]
public class Player
{
    public ulong ClientId;
    public int Index;
    public string DisplayName;
    public NetworkFungal Fungal;
    public float Score => Fungal.Score;
    public int Lives => Fungal.Lives;
    public bool IsWinner = false;

    public Player(ulong clientId, int playerIndex, string displayName, NetworkFungal fungal)
    {
        ClientId = clientId;
        Index = playerIndex;
        DisplayName = displayName;
        Fungal = fungal;
    }
}

//todo: rename
[CreateAssetMenu]
public class GameReference : ScriptableObject
{
    [SerializeField] private Player clientPlayer;
    [SerializeField] private List<Player> players;
    [SerializeField] private MultiplayerReference multiplayer;

    public bool isComplete;
    public GameMode gameMode;

    public Player ClientPlayer => clientPlayer;
    public List<Player> Players => players;

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnClientPlayerAdded;
    public event UnityAction<Player> OnPlayerAdded;
    public event UnityAction OnAllPlayersAdded;
    public event UnityAction<int, int> OnKill;
    public event UnityAction<Player> OnSelfDestruct;

    public void Initialize()
    {
        clientPlayer = null;
        players = new List<Player>();
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

        var addedPlayer = new Player(clientId, playerIndex, playerName.ToString(), networkFungal);
        Players.Add(addedPlayer);

        // Sort the list based on the player index
        Players.Sort((player1, player2) => player1.Index.CompareTo(player2.Index));

        if (networkFungal.IsOwner && !networkFungal.IsAI)
        {
            clientPlayer = addedPlayer;
            Debug.Log("OnClientPlayerAdded");
            OnClientPlayerAdded?.Invoke();
        }

        OnPlayerAdded?.Invoke(addedPlayer);
        if (multiplayer.LobbyPlayers.Count == Players.Count)
        {
            OnAllPlayersAdded?.Invoke();
        }

        if (gameMode== GameMode.PARTY)
        {
            addedPlayer.Fungal.OnScoreUpdated += Fungal_OnScoreUpdated;
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
            TriggerWin();
        }
        else if (Players.Count > 1 && alivePlayers.Count == 1)
        {
            var winner = alivePlayers[0];
            winner.IsWinner = true;
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        isComplete = true;
        clientPlayer.Fungal.Movement.Stop();
        OnGameComplete?.Invoke();
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
