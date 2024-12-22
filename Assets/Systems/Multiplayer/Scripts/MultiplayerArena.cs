using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private int deadPlayerCount = 0;

    public List<Transform> Players { get; private set; }
    public List<Transform> Spores { get; private set; }
    public List<Transform> SporePositions { get; private set; }
    public Vector3 PlayerSpawnPosition { get; private set; }

    public int MushroomsCollected { get; private set; }
    public int MushroomRequirement => Spores.Count;

    public event UnityAction OnAllMushroomsCollected;
    public event UnityAction OnAllPlayersDead;
    public event UnityAction OnAllPlayersSpawned;
    public event UnityAction OnIntroComplete;

    public void Initialize(Vector3 playerSpawnPosition, List<Transform> spores, List<Transform> sporePositions)
    {
        Players = new List<Transform>();
        Spores = spores;
        SporePositions = sporePositions;
        PlayerSpawnPosition = playerSpawnPosition;
        MushroomsCollected = 0;
        deadPlayerCount = 0;
    }

    public void RegisterPlayer(Transform player)
    {
        Players.Add(player);

        if (multiplayer.JoinedLobby != null)
        {
            if (multiplayer.JoinedLobby.Players.Count == Players.Count)
            {
                OnAllPlayersSpawned?.Invoke();
            }
        }
    }

    public void IncrementMushroomsCollected()
    {
        MushroomsCollected++;
        Debug.Log(MushroomsCollected);
        if (MushroomsCollected == MushroomRequirement)
        {
            OnAllMushroomsCollected?.Invoke();
        }
    }

    public void IncrementDeadPlayerCount()
    {
        deadPlayerCount++;
        if (deadPlayerCount == Players.Count)
        {
            OnAllPlayersDead?.Invoke();
        }
    }

    public void InvokeIntroComplete()
    {
        OnIntroComplete?.Invoke();
    }
}
