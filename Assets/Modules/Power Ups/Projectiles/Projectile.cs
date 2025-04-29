using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float hitStun = 0.5f;

    [SerializeField] private bool useTrajectory;
    [SerializeField] private bool isExplosion;

    public FungalController Fungal { get; private set; }
    public bool InMotion { get; private set; }

    private Movement movement;
    private HitDetector hitDetector;

    private void Awake()
    {
        hitDetector = GetComponent<HitDetector>();
        movement = GetComponent<Movement>();
    }

    public void Initialize(FungalController fungal, bool useTrajectory)
    {
        Fungal = fungal;
        this.useTrajectory = useTrajectory;
        transform.GetChild(0).localScale = Vector3.zero;
    }

    public void ShootProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;

        if (useTrajectory)
        {
            movement.SetTrajectoryMovement(targetPosition);
        }
        else
        {
            movement.SetTargetPosition(targetPosition);
        }

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
            InMotion = true;
            yield return movement.ScaleOverTime(0.1f, 1f);

            yield return new WaitUntil(() => hasHitOrArrived || movement.IsAtDestination);
            hasHitOrArrived = true;

            yield return movement.ScaleOverTime(0.5f, 0);
            InMotion = false;
        }

        IEnumerator HandleExplosionDamage()
        {
            yield return new WaitUntil(() => movement.IsAtDestination);

            List<FungalController> hits = new List<FungalController>();

            while (true)
            {
                hitDetector.CheckFungalHits(transform.localScale.x / 2f, damage, hitStun, Fungal,
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
            yield return new WaitUntil(() => movement.IsAtDestination);
            Debug.Log($"landed {targetPosition}");
            yield return movement.ScaleOverTime(0.2f, 0, 2f * 2);
            yield return new WaitForSeconds(0.5f);
            yield return movement.ScaleOverTime(0.1f, 2f * 2, 0);
        }

        if (isExplosion)
        {
            StartCoroutine(HandleExplosionDamage());
            StartCoroutine(HandleExplosionSize());
        }
        else
        {
            StartCoroutine(HandleSimpleDamage());
            StartCoroutine(HandleSimpleSize());
        }
    }
}
