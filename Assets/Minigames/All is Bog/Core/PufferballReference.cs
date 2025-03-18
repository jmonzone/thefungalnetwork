using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerData
{
    public string name;
    public int index;
    public NetworkFungal fungal;
    private float score = 0;

    public float Score => score;
    public bool IsWinner => score >= 1000f;

    public event UnityAction OnScoreUpdated;


    public void SetScore(float value)
    {
        score = value;
        OnScoreUpdated?.Invoke();
    }

    public void AddToScore(float value)
    {
        SetScore(score + value);
    }
}

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    public FishingRodProjectile FishingRod { get; private set; }
    public PlayerData Player { get; private set; }
    public List<PlayerData> Players { get; private set; }

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnFishingRodUpdated;
    public event UnityAction OnPufferfishUpdated;

    public event UnityAction OnPlayerRegistered;
    public event UnityAction<NetworkFungal, int> OnPlayerDefeated;
    public event UnityAction OnRespawnStart;
    public event UnityAction OnRespawnComplete;

    public void Initialize()
    {
        Player = null;
        Players = new List<PlayerData>();
        OnPufferfishUpdated?.Invoke();
    }

    public void RegisterFishingRod(FishingRodProjectile fishingRod)
    {
        FishingRod = fishingRod;
        OnFishingRodUpdated?.Invoke();
    }

    public void RegisterPlayer(NetworkFungal fungal, int index)
    {
        // Check if a player with the same index already exists
        if (Players.Any(player => player.index == index))
        {
            Debug.Log($"Player with index {index} already exists. Skipping registration.");
            return; // Skip if player already exists
        }

        // Create new player data
        var playerData = new PlayerData
        {
            index = index,
            fungal = fungal
        };

        // Register the player if they are the owner
        if (fungal.IsOwner)
        {
            Player = playerData;
            Debug.Log("OnPlayerRegistered");
            OnPlayerRegistered?.Invoke();
        }

        // Add the player to the list
        Players.Add(playerData);

        // Sort the list based on the player index
        Players.Sort((player1, player2) => player1.index.CompareTo(player2.index));

        // Subscribe to events
        playerData.OnScoreUpdated += () => OnScoreUpdated?.Invoke();

        fungal.Health.OnHealthChanged += source =>
        {
            if (fungal.PlayerIndex != source)
            {
                var damageSource = 35f;

                if (source > -1) Players[source].AddToScore(damageSource);

                OnScoreUpdated?.Invoke();
            }
        };

        fungal.OnDeath += source =>
        {
            OnPlayerDeath(fungal, source);
        };

        OnScoreUpdated?.Invoke();
    }


    public void OnPlayerDeath(NetworkFungal fungal, int source)
    {
        Debug.Log("OnPlayerDeath source " + source);

        if (fungal.PlayerIndex != source)
        {
            var killScore = 250f;

            if (source > -1) Players[source].AddToScore(killScore);

            OnScoreUpdated?.Invoke();

            foreach (var player in Players)
            {
                if (player.IsWinner)
                {
                    OnGameComplete?.Invoke();
                    return;
                }
            }
        }

        if (fungal.IsOwner)
        {
            Debug.Log("OnPlayerDeath.networkObject.IsOwner");

            fungal.StartCoroutine(RespawnRoutine(fungal));
        }
    }

    public float RemainingRespawnTime { get; private set; }

    // You can adjust this in the inspector or hardcode it.
    [SerializeField] private float respawnDuration = 5f;

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
