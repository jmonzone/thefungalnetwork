using UnityEngine;

[CreateAssetMenu]
public class ShruneItem : Item
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float maxDistance;

    public Projectile ProjectilePrefab => projectilePrefab;
    public float MaxDistance => maxDistance;
}
