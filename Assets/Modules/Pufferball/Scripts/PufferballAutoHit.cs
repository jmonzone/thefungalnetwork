using UnityEngine;

public class PufferballAutoHit : MonoBehaviour
{
    [SerializeField] private float hitForce = 300f;

    private bool canHit = true;

    private void Awake()
    {
        var detectCollider = GetComponent<DetectCollider>();
        detectCollider.OnColliderDetected += collider =>
        {
            if (canHit)
            {
                var hitDirection = collider.bounds.ClosestPoint(transform.position) - transform.position;
                hitDirection.y = 0;

                var pufferball = collider.GetComponentInParent<PufferballController>();
                pufferball.Rigidbody.AddForce(hitForce * hitDirection.normalized);
                canHit = false;
                Invoke(nameof(ResetHitTimer), 3f);
            }
        };
    }

    private void ResetHitTimer()
    {
        canHit = true;
    }
}
