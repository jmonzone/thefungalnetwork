using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    public FishingRodProjectile FishingRod { get; private set; }
    public NetworkFungal Player { get; private set; }
    public List<NetworkFungal> Players { get; private set; }

    public int CurrentScore { get; private set; }
    public int OpponentScore { get; private set; }

    public bool IsWinner { get; private set; }

    public event UnityAction OnScoreUpdated;
    public event UnityAction OnGameComplete;

    public event UnityAction OnFishingRodUpdated;
    public event UnityAction OnPufferfishUpdated;
    public event UnityAction<ulong> OnPlayerDefeated;

    public void Initialize()
    {
        CurrentScore = 0;
        OpponentScore = 0;
        IsWinner = false;
        Player = null;
        Players = new List<NetworkFungal>();
        OnPufferfishUpdated?.Invoke();
    }

    public void RegisterFishingRod(FishingRodProjectile fishingRod)
    {
        FishingRod = fishingRod;
        OnFishingRodUpdated?.Invoke();
    }

    public void RegisterPlayer(NetworkFungal player)
    {
        if (player.IsOwner) Player = player;
        Players.Add(player);
        player.OnHealthDepleted += () =>
        {
            OnPlayerDefeated?.Invoke(player.NetworkObjectId);
        };
    }

    public void UpdateScore(NetworkObject networkObject)
    {
        if (networkObject.IsOwner) OpponentScore++;
        else CurrentScore++;

        OnScoreUpdated?.Invoke();

        IsWinner = CurrentScore >= 3;

        if (IsWinner || OpponentScore >= 3)
        {
            OnGameComplete?.Invoke();
        }

        if (networkObject.IsOwner)
        {
            var networkFungal = networkObject.GetComponent<NetworkFungal>();
            networkFungal.RespawnServerRpc();
        }
    }
}
