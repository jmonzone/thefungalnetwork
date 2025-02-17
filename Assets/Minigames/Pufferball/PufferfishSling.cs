using UnityEngine;
using UnityEngine.Events;

public class PufferfishSling : MonoBehaviour
{
    [SerializeField] private float range = 5f;

    private Vector3 targetPosition;
    private Movement movement;

    public event UnityAction OnSlingComplete;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public void Sling(Vector3 direction)
    {
        enabled = true;
        targetPosition = transform.position + direction * range;
        targetPosition.y = 0; // Keep on the ground
        movement.SetTargetPosition(targetPosition); // Move towards the target
    }

    private void Update()
    {
        // Handle slung movement (already handled in Sling method)
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            enabled = false;
            OnSlingComplete?.Invoke();
        }
    }
}
