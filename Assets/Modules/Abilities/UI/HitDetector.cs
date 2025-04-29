using System;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    private float radius = 1f;

    public void CheckHits(float radius, System.Action<Collider> onHit)
    {
        this.radius = radius;

        // Get all colliders in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hitColliders)
        {
            // Compare to the collider's origin (usually the object's transform position)
            Vector3 colliderOrigin = hit.transform.position;

            // Example comparison: Check if colliderOrigin is within a certain condition
            // This is just an example condition; adjust to your needs
            float distanceFromOrigin = Vector3.Distance(transform.position, colliderOrigin);

            if (distanceFromOrigin <= radius)
            {
                // Invoke the callback for valid hits
                onHit?.Invoke(hit);
            }
        }
    }

    public void CheckFungalHits(float radius, float damage, float hitStun, FungalController source, System.Action<FungalController> onHit, Func<FungalController, bool> isValid = null)
    {
        this.radius = radius;

        // Get all colliders in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hitColliders)
        {
            // Compare to the collider's origin (usually the object's transform position)
            Vector3 colliderOrigin = hit.transform.position;

            // Example comparison: Check if colliderOrigin is within a certain condition
            // This is just an example condition; adjust to your needs
            float distanceFromOrigin = Vector3.Distance(transform.position, colliderOrigin);

            if (distanceFromOrigin <= radius)
            {
                var targetFungal = hit.GetComponent<FungalController>();

                if (targetFungal == null) continue;
                if (targetFungal == source) continue;
                if (targetFungal.IsDead) continue;
                if (isValid != null && !isValid(targetFungal)) continue;

                Debug.Log("damaged");
                targetFungal.ModifySpeed(0f, hitStun, showStunAnimation: false);
                targetFungal.Health.Damage(damage, source.Id);

                // Invoke the callback for valid hits
                onHit?.Invoke(targetFungal);
            }
        }
    }


    // Optional: Draw the hit radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}