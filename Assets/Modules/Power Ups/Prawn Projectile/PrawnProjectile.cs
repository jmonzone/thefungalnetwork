using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Power Ups/Prawn Projectile")]
public class PrawnProjectile : DirectionalAbility
{
    [SerializeField] private Movement projectilePrefab;
    [SerializeField] private float range;

    public override float Range => range;
    public override bool UseTrajectory => false;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility();

        var projectile = Instantiate(projectilePrefab, Fungal.transform.position, Quaternion.identity);
        projectile.SetTargetPosition(targetPosition);

        Fungal.StartCoroutine(ShootProjectileRoutine(projectile));
    }

    private IEnumerator ShootProjectileRoutine(Movement projectile)
    {
        yield return new WaitUntil(() => projectile.IsAtDestination);
        yield return projectile.ScaleOverTime(0.5f, 0);
        projectile.gameObject.SetActive(false);
    }
}
