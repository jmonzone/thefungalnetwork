using UnityEngine;

[CreateAssetMenu]
public class ShruneItem : Item
{
    [SerializeField] private Projectile projectilePrefab;

    public Projectile ProjectilePrefab => projectilePrefab;
}
