using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionProjectile : MonoBehaviour, IProjectileBehavior
{
    private Projectile projectile;

    public void Initialize(Projectile projectile)
    {
        this.projectile = projectile;
    }

    public void Shoot(Vector3 startPosition, Vector3 targetPosition)
    {
        projectile.Movement.SetTrajectoryMovement(targetPosition);

        projectile.StartCoroutine(ExplosionDamage());
        projectile.StartCoroutine(ExplosionSize());
    }

    private IEnumerator ExplosionDamage()
    {
        yield return new WaitUntil(() => projectile.Movement.IsAtDestination);

        List<FungalController> hits = new List<FungalController>();

        while (projectile.Movement.ScaleTransform.localScale.x > 0)
        {
            projectile.hitDetector.CheckFungalHits(
                projectile.Movement.ScaleTransform.localScale.x / 2f,
                projectile.Damage, projectile.HitStun,
                projectile.Fungal,
                hit => hits.Add(hit),
                fungal => !hits.Contains(fungal)
            );

            yield return null;
        }
    }

    private IEnumerator ExplosionSize()
    {
        projectile.InMotion = true;

        yield return projectile.Movement.ScaleOverTime(0.1f, 1f);
        yield return new WaitUntil(() => projectile.Movement.IsAtDestination);
        projectile.CompleteTrajectory();

        yield return projectile.Movement.ScaleOverTime(0.2f, 0, 2f * projectile.Radius);
        yield return new WaitForSeconds(0.5f);
        yield return projectile.Movement.ScaleOverTime(0.1f, 2f * projectile.Radius, 0);

        projectile.InMotion = false;
    }
}
