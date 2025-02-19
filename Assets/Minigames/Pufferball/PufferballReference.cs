using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PufferballReference : ScriptableObject
{
    [SerializeField] private Pufferfish pufferfish;

    public FishingRodProjectile FishingRod { get; private set; }
    public ulong PufferfishNetworkId { get; private set; }
    public List<NetworkFungal> Players { get; private set; }

    public event UnityAction OnFishingRodUpdated;
    public event UnityAction OnPufferfishUpdated;
    public event UnityAction<ulong> OnPlayerDefeated;

    public void Initialize(Pufferfish pufferfish)
    {
        Players = new List<NetworkFungal>();
        PufferfishNetworkId = pufferfish.NetworkObjectId;
        OnPufferfishUpdated?.Invoke();
    }

    public void RegisterFishingRod(FishingRodProjectile fishingRod)
    {
        FishingRod = fishingRod;
        OnFishingRodUpdated?.Invoke();
    }

    public void RegisterPlayer(NetworkFungal player)
    {
        Players.Add(player);
        player.OnHealthDepleted += () =>
        {
            OnPlayerDefeated?.Invoke(player.OwnerClientId);
        };
    }

    public NetworkObject GetPufferfish()
    {
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(PufferfishNetworkId, out var obj) ? obj : null;
    }

}
