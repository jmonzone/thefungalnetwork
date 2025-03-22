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


    // Optional: Draw the hit radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}