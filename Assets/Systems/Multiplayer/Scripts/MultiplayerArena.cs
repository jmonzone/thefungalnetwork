using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    public List<Transform> Players { get; private set; }
    public Vector3 PlayerSpawnPosition { get; private set; }

    public int MushroomsCollected { get; private set; }
    public const int MushroomRequirement = 5;

    public event UnityAction OnAllMushroomsCollected;

    public void Initialize(Vector3 playerSpawnPosition)
    {
        Players = new List<Transform>();
        PlayerSpawnPosition = playerSpawnPosition;
        MushroomsCollected = 0;
    }

    public void RegisterPlayer(Transform player)
    {
        Players.Add(player);
    }

    public void IncrementMushroomsCollected()
    {
        MushroomsCollected++;
        if (MushroomsCollected == 5)
        {
            OnAllMushroomsCollected?.Invoke();
        }
    }
}
