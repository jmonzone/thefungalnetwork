using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    public List<Transform> Players { get; private set; }
    public Vector3 PlayerSpawnPosition { get; private set; }

    public void Initialize(Vector3 playerSpawnPosition)
    {
        Players = new List<Transform>();
        PlayerSpawnPosition = playerSpawnPosition;
    }

    public void RegisterPlayer(Transform player)
    {
        Players.Add(player);
    }
}
