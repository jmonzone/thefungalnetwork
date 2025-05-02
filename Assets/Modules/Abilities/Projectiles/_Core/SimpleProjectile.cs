using System.Collections;
using UnityEngine;

public class SimpleProjectile: MonoBehaviour, IProjectileBehavior
{
    private Projectile projectile;
    private bool hasHitOrArrived = false;

    public void Initialize(Projectile projectile)
    {
        this.projectile = projectile;
    }

    public void Shoot(Vector3 startPosition, Vector3 targetPosition)
    {
        projectile.Movement.SetTargetPosition(targetPosition);

        StopAllCoroutines();

        projectile.StartCoroutine(SimpleDamage());
        projectile.StartCoroutine(SimpleSize());
    }

    private IEnumerator SimpleDamage()
    {
        while (!hasHitOrArrived)
        {
            projectile.hitDetector.CheckFungalHits(1f, projectile.Damage, projectile.HitStun, projectile.Fungal,
                hit => hasHitOrArrived = true);

            yield return null;
        }
    }

    private IEnumerator SimpleSize()
    {
        //Debug.Log("Sized");
        projectile.InMotion = true;
        yield return projectile.Movement.ScaleOverTime(0.1f, 1f);

        yield return new WaitUntil(() => hasHitOrArrived || projectile.Movement.IsAtDestination);
        hasHitOrArrived = true;

        yield return projectile.Movement.ScaleOverTime(0.5f, 0);
        projectile.InMotion = false;
    }
}
