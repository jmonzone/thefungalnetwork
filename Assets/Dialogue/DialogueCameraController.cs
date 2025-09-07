using UnityEngine;

public class DialogueCameraController : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float sideDistance = 2f;   // distance to side
    [SerializeField] private float forwardBias = 0.5f; // how much toward front (0 = pure 90°, 1 = fully forward)
    [SerializeField] private float heightOffset = 1.2f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!playerReference || !playerReference.Player) return;

        Transform player = playerReference.Player.transform;
        Transform target = transform.parent; // parent character

        // Horizontal vector from target to player
        Vector3 toPlayer = player.position - target.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.001f) return;
        toPlayer.Normalize();

        // Left/right perpendicular offsets
        Vector3 left = Vector3.Cross(Vector3.up, toPlayer).normalized;
        Vector3 right = Vector3.Cross(toPlayer, Vector3.up).normalized;

        // Decide side based on main camera position
        float leftDot = Vector3.Dot(mainCamera.transform.position - target.position, left);
        float rightDot = Vector3.Dot(mainCamera.transform.position - target.position, right);
        Vector3 chosenSide = (leftDot > rightDot) ? left : right;

        // Apply forward bias for "11 o'clock / 1 o'clock" effect
        Vector3 offsetDir = (chosenSide + toPlayer * forwardBias).normalized;

        // Position the rig relative to target
        transform.position = target.position + offsetDir * sideDistance + Vector3.up * heightOffset;

        // Look at target
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }
}
