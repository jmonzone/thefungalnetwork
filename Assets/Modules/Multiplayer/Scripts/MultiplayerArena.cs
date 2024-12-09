using UnityEngine;

[CreateAssetMenu]
public class MultiplayerArena : ScriptableObject
{
    public Vector3 PlayerSpawnPosition { get; private set; }
    public Vector3 CrocodileSpawnPosition { get; private set; }

    public void Initialize(Vector3 playerSpawnPosition, Vector3 crocodileSpawnPosition)
    {
        PlayerSpawnPosition = playerSpawnPosition;
        CrocodileSpawnPosition = crocodileSpawnPosition;
    }
}
