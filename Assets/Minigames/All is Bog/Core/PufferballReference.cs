using System;
using System.Collections;
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

    public Player(ulong clientId, int index, NetworkFungal fungal, bool isAI = false)
    {
        ClientId = clientId;
        Index = index;
        Fungal = fungal;
        IsAI = isAI;
    }

    public void AddToScore(float value)
    {
        Score += value;
        OnScoreUpdated?.Invoke();
    }
}

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    [SerializeField] private Player clientPlayer;
    [SerializeField] private List<Player> players;
    [SerializeField] private float respawnDuration = 5f;

    public Player ClientPlayer => clientPlayer;
    public List<Player> Players => players;

    public float RemainingRespawnTime { get; private set; }

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnClientPlayerAdded;
    public event UnityAction<Player> OnPlayerAdded;

    public event UnityAction OnRespawnStart;
    public event UnityAction OnRespawnComplete;

    public void Initialize()
    {
        clientPlayer = null;
        players = new List<Player>();
    }

    public void AddPlayer(ulong clientId, int index, NetworkFungal networkFungal, bool isAi)
    {
        // Check if a player with the same index already exists
        if (Players.Any(player => player.Index == index))
        {
            //Debug.Log($"Player with index {index} already exists. Skipping registration.");
            return; // Skip if player already exists
        }

        var addedPlayer = new Player(clientId, index, networkFungal, isAi);
        Players.Add(addedPlayer);

        // Sort the list based on the player index
        Players.Sort((player1, player2) => player1.Index.CompareTo(player2.Index));

        if (networkFungal.IsOwner)
        {
            clientPlayer = addedPlayer;
            OnClientPlayerAdded?.Invoke();
        }

        OnPlayerAdded?.Invoke(addedPlayer);

        addedPlayer.OnScoreUpdated += () =>
        {
            OnScoreUpdated?.Invoke();

            foreach (var player in Players)
            {
                if (player.IsWinner)
                {
                    OnGameComplete?.Invoke();
                    return;
                }
            }
        };

        addedPlayer.Fungal.GetComponent<FishPickup>().OnFishPickedUp += () =>
        {
            Players[index].AddToScore(20f);
        };

        addedPlayer.Fungal.Health.OnHealthChanged += source =>
        {
            ValidateAndAddScore(index, source, 35f);
        };

        addedPlayer.Fungal.OnDeath += source =>
        {
            ValidateAndAddScore(index, source, 250f);

            if (addedPlayer.Fungal.IsOwner)
            {
                Debug.Log("OnPlayerDeath.networkObject.IsOwner");

                addedPlayer.Fungal.StartCoroutine(RespawnRoutine(addedPlayer.Fungal));
            }
        };
    }


    private void ValidateAndAddScore(int index, int source, float value)
    {
        if (index != source && source > -1f)
        {
            Players[source].AddToScore(value);
        }

    }
    private IEnumerator RespawnRoutine(NetworkFungal fungal)
    {
        OnRespawnStart?.Invoke();
        RemainingRespawnTime = respawnDuration;

        while (RemainingRespawnTime > 0f)
        {
            yield return null; // Wait for next frame
            RemainingRespawnTime -= Time.deltaTime;
        }

        RemainingRespawnTime = 0f;

        fungal.RespawnServerRpc();

        OnRespawnComplete?.Invoke();
    }
}
