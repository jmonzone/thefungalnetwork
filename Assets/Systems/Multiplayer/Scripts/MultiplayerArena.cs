using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    public List<Transform> Players { get; private set; }
    public Vector3 PlayerSpawnPosition { get; private set; }
    public Vector3 CrocodileSpawnPosition { get; private set; }

    public void Initialize(Vector3 playerSpawnPosition, Vector3 crocodileSpawnPosition)
    {
        Players = new List<Transform>();
        PlayerSpawnPosition = playerSpawnPosition;
        CrocodileSpawnPosition = crocodileSpawnPosition;
    }

    public void RegisterPlayer(Transform player)
    {
        Players.Add(player);
    }
}
