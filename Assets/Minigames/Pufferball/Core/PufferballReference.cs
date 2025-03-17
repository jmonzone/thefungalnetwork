using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    public FishingRodProjectile FishingRod { get; private set; }
    public PlayerData Player { get; private set; }
    public List<PlayerData> Players { get; private set; }

    public class PlayerData
    {
        public NetworkFungal fungal;
        public float score;
        public bool IsWinner => score >= 100f;
    }

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnFishingRodUpdated;
    public event UnityAction OnPufferfishUpdated;

    public event UnityAction OnPlayerRegistered;
    public event UnityAction<ulong, int> OnPlayerDefeated;
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

    public void RegisterPlayer(NetworkFungal fungal)
    {
        var playerData = new PlayerData
        {
            fungal = fungal
        };

        if (fungal.IsOwner)
        {
            Player = playerData;
            Debug.Log("OnPlayerRegistered");
            OnPlayerRegistered?.Invoke();
        }

        Players.Add(playerData);

        foreach(var player in Players)
        {
            player.score = 100f / Players.Count;
        }

        fungal.OnDeath += source =>
        {
            OnPlayerDefeated?.Invoke(fungal.NetworkObjectId, source);
        };
    }

    public void OnPlayerDeath(NetworkObject networkObject, int source)
    {
        var killScore = 20f;

        var i = 0;

        if (source > -1)
        {
            Players[source].score += killScore;
        }

        PlayerData winner = null;
        foreach (var player in Players)
        {
            if (player.fungal.NetworkObject == networkObject)
            {
                player.score -= killScore;
            }

            if (player.IsWinner) winner = player;

            i++;
        }

        OnScoreUpdated?.Invoke();

        if (winner == null)
        {
            networkObject.StartCoroutine(RespawnRoutine(networkObject));
        }
        else
        {
            OnGameComplete?.Invoke();
        }
    }

    public float RemainingRespawnTime { get; private set; }

    // You can adjust this in the inspector or hardcode it.
    [SerializeField] private float respawnDuration = 5f;

    private IEnumerator RespawnRoutine(NetworkObject networkObject)
    {
        OnRespawnStart?.Invoke();
        RemainingRespawnTime = respawnDuration;

        while (RemainingRespawnTime > 0f)
        {
            yield return null; // Wait for next frame
            RemainingRespawnTime -= Time.deltaTime;
        }

        RemainingRespawnTime = 0f;

        var networkFungal = networkObject.GetComponent<NetworkFungal>();

        networkFungal.RespawnServerRpc();

        OnRespawnComplete?.Invoke();
    }
}
