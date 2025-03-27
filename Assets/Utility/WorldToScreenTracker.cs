using UnityEngine;

public class WorldToScreenTracker : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 20f;  // Adjust smoothness

    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Convert target world position to screen space
        var targetPosition = mainCamera.WorldToScreenPoint(target.position);

        // Smoothly transition the tracker position
        transform.position = targetPosition;
    }

    private void LateUpdate()
    {
        // Convert target world position to screen space
        var targetPosition = mainCamera.WorldToScreenPoint(target.position);

        // Smoothly transition the tracker position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed * Time.deltaTime);
    }
}
