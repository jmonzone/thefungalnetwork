using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    private float radius = 1f;

    public void CheckHits(float radius, System.Action<Collider> onHit)
    {
        this.radius = radius;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hitColliders)
        {
            onHit?.Invoke(hit);
        }
    }

    // Optional: Draw the hit radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}