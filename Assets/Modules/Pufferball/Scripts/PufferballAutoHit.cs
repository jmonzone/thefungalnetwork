using UnityEngine;

public class PufferballAutoHit : MonoBehaviour
{
    [SerializeField] private float hitRadius = 1.25f;
    [SerializeField] private float hitForce = 250f;
    [SerializeField] private LayerMask layerMask;

    private bool canHit = true;

    private void Update()
    {
        if (canHit)
        {
            var colliders = Physics.OverlapSphere(transform.position, hitRadius, layerMask);
            if (colliders.Length > 0)
            {
                var collider = colliders[0];
                var pufferball = collider.GetComponentInParent<PufferballController>();
                var hitDirection = collider.bounds.ClosestPoint(transform.position) - transform.position;
                hitDirection.y = 0;
                pufferball.Rigidbody.AddForce(hitForce * hitDirection.normalized);
                canHit = false;
                Invoke(nameof(ResetHitTimer), 3f);
            }
        }
    }

    private void ResetHitTimer()
    {
        canHit = true;
    }
}
