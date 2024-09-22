using UnityEngine;

public class PufferballAutoHit : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private bool canHit = true;

    private void Update()
    {
        if (canHit)
        {

            var colliders = Physics.OverlapSphere(transform.position, 1, layerMask);
            if (colliders.Length > 0)
            {
                Debug.Log("hitting");

                var movement = colliders[0].GetComponentInParent<MovementController>();
                var hitDirection = transform.forward;
                hitDirection.y = 0;
                movement.SetDirection(hitDirection.normalized);
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
