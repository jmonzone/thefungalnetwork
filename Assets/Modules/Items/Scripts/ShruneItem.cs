using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu]
public class ShruneItem : Item
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private NetworkObject networkPrefab;
    [SerializeField] private float maxDistance;

    public Projectile ProjectilePrefab => projectilePrefab;
    public NetworkObject NetworkPrefab => networkPrefab;
    public float MaxDistance => maxDistance;
}
