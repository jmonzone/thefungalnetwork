using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu]
public class ShruneItem : Item
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private NetworkObject networkPrefab;
    [SerializeField] private float maxDistance;
    [SerializeField] private float cooldown;
    [SerializeField] private float speed;
    [SerializeField] private Sprite abilityIcon;

    public Projectile ProjectilePrefab => projectilePrefab;
    public NetworkObject NetworkPrefab => networkPrefab;
    public float MaxDistance => maxDistance;
    public float Cooldown => cooldown;
    public float Speed => speed;
    public Sprite AbilityIcon => abilityIcon;
}
