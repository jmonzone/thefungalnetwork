using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Power Ups/Prawn Projectile")]
public class PrawnProjectile : DirectionalAbility
{
    [SerializeField] private Movement projectilePrefab;
    [SerializeField] private float range;

    [SerializeField] private float damage;
    [SerializeField] private float hitStun = 0.5f;


    [SerializeField] private bool useTrajectory;
    [SerializeField] private bool isExplosion;

    public override float Range => range;
    public override bool UseTrajectory => useTrajectory;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility();

        var projectile = Instantiate(projectilePrefab, Fungal.transform.position, Quaternion.identity);

        if (useTrajectory)
        {
            projectile.SetTrajectoryMovement(targetPosition);
        }
        else
        {
            projectile.SetTargetPosition(targetPosition);
        }

        var hitDetector = projectile.GetComponent<HitDetector>();

        bool hasHitOrArrived = false;

        IEnumerator HandleSimpleDamage()
        {
            while (!hasHitOrArrived)
            {
                hitDetector.CheckFungalHits(1f, damage, hitStun, Fungal, hit =>
                {
                    hasHitOrArrived = true;
                });

                yield return null;
            }
        }

        IEnumerator HandleSimpleSize()
        {
            // Wait until it either hits or reaches destination
            yield return new WaitUntil(() => hasHitOrArrived || projectile.IsAtDestination);
            hasHitOrArrived = true;

            yield return projectile.ScaleOverTime(0.5f, 0);
            projectile.gameObject.SetActive(false);
        }

        IEnumerator HandleExplosionDamage()
        {
            yield return new WaitUntil(() => projectile.IsAtDestination);

            List<FungalController> hits = new List<FungalController>();

            while (true)
            {
                hitDetector.CheckFungalHits(projectile.transform.localScale.x / 2f, damage, hitStun, Fungal,
                    onHit: hit =>
                    {
                        hits.Add(hit);
                    },
                    isValid: (fungal) =>
                    {
                        return !hits.Contains(fungal);
                    });

                yield return null;
            }
        }

        IEnumerator HandleExplosionSize()
        {
            // Wait until it either hits or reaches destination
            yield return new WaitUntil(() => projectile.IsAtDestination);
            Debug.Log($"landed {targetPosition}");
            yield return projectile.ScaleOverTime(0.2f, 0, 2f * radius);
            yield return new WaitForSeconds(0.5f);
            yield return projectile.ScaleOverTime(0.1f, 2f * radius, 0);
            projectile.gameObject.SetActive(false);
        }

        if (isExplosion)
        {
            Fungal.StartCoroutine(HandleExplosionDamage());
            Fungal.StartCoroutine(HandleExplosionSize());
        }
        else
        {
            Fungal.StartCoroutine(HandleSimpleDamage());
            Fungal.StartCoroutine(HandleSimpleSize());
        }
    }
}
