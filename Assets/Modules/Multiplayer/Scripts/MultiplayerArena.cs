using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    [SerializeField] private MultiplayerReference multiplayer;

    public List<Transform> Players { get; private set; }
    public List<Transform> SpawnPositions { get; private set; }

    public event UnityAction OnAllPlayersSpawned;

    public void Initialize(List<Transform> spawnPositions)
    {
        Players = new List<Transform>();
        SpawnPositions = spawnPositions;
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
        else
        {
            OnAllPlayersSpawned?.Invoke();
        }
    }
}
