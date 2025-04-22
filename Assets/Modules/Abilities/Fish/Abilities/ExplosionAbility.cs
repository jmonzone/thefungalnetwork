using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Fish/Ability/Explosion")]
public class ExplosionAbility : DirectionalAbility, IMovementAbility
{
    [SerializeField] private PufferfishExplosion explosion;

    [SerializeField] private float damage = 3f;
    [SerializeField] private float range = 4f;

    public override Vector3 DefaultTargetPosition => Fungal.transform.position + Fungal.transform.forward * range;

    public override bool UseTrajectory => true;

    public override float Range => range;

    public event UnityAction OnExplosionSpawned;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility(targetPosition);

        Fungal.SpawnObject(explosion.gameObject, targetPosition);

    }
}
