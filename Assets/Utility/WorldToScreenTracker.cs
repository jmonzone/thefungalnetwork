using UnityEngine;

public class WorldToScreenTracker : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform anchor;

    [SerializeField] private float smoothSpeed = 20f;  // Adjust smoothness

    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        mainCamera = Camera.main;

        // Convert target world position to screen space
        var targetPosition = mainCamera.WorldToScreenPoint(target.position);

        // Smoothly transition the tracker position
        transform.position = targetPosition;

        target.parent = null;
    }

    private void LateUpdate()
    {
        if (!target) return;

        target.position = anchor.position + Vector3.up * 2.4f;

        // Convert target world position to screen space
        var targetPosition = mainCamera.WorldToScreenPoint(target.position);

        // Smoothly transition the tracker position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed * Time.deltaTime);
    }
}
