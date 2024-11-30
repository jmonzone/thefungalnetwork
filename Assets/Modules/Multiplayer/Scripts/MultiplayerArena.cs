using UnityEngine;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    public Vector3 SpawnPosition1 { get; private set; }
    public Vector3 SpawnPosition2 { get; private set; }

    public void Initialize(Vector3 spawnPosition1, Vector3 spawnPosition2)
    {
        SpawnPosition1 = spawnPosition1;
        SpawnPosition2 = spawnPosition2;
    }
}
