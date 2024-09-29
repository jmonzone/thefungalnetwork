using UnityEngine;
using UnityEngine.Events;

public class DetectCollider : MonoBehaviour
{
    [SerializeField] private float radius = 1.25f;
    [SerializeField] private LayerMask layerMask;

    public event UnityAction<Collider> OnColliderDetected;

    private void Update()
    {
        var colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        if (colliders.Length > 0)
        {
            var collider = colliders[0];
            OnColliderDetected?.Invoke(collider);
        }
    }
}
