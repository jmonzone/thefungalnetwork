using UnityEngine;

public class PufferballAutoHit : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private bool canHit = true;

    private void Update()
    {
        if (canHit)
        {
            var colliders = Physics.OverlapSphere(transform.position, 1.5f, layerMask);
            if (colliders.Length > 0)
            {
                var pufferball = colliders[0].GetComponentInParent<PufferballController>();
                var hitDirection = transform.forward;
                hitDirection.y = 0;
                pufferball.Rigidbody.AddForce(500f * hitDirection);
                canHit = false;
                Invoke(nameof(ResetHitTimer), 2f);
            }
        }
    }

    private void ResetHitTimer()
    {
        canHit = true;
    }
}
