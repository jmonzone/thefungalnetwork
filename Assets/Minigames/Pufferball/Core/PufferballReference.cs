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

    public event UnityAction OnPlayerRegistered;
    public event UnityAction<ulong> OnPlayerDefeated;
    public event UnityAction OnRespawnStart;
    public event UnityAction OnRespawnComplete;

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
        if (player.IsOwner)
        {
            Player = player;
            Debug.Log("OnPlayerRegistered");
            OnPlayerRegistered?.Invoke();
        }

        Players.Add(player);
        player.OnDeath += () =>
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
            return;
        }

        if (networkObject.IsOwner)
        {
            networkObject.StartCoroutine(RespawnRoutine(networkObject));
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
