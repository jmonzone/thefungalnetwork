using UnityEngine;

public class CameraPanController : MonoBehaviour
{
    public float panSpeed = 0.1f;
    public float smoothTime = 0.2f;
    public bool invertX = false;
    public bool invertY = false;

    public Vector2 panLimitX = new Vector2(-5f, 5f);  // left/right bounds
    public Vector2 panLimitY = new Vector2(-5f, 5f);  // forward/back bounds

    private Vector3 lastPanPosition;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private bool isPanning;

    public Vector2 inputDelta; 

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        inputDelta = Vector2.zero;

        // Touch Input
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                isPanning = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                inputDelta = touch.position - (Vector2)lastPanPosition;
                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }
        // Mouse Input
        else if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
            isPanning = true;
        }
        else if (Input.GetMouseButton(0) && isPanning)
        {
            inputDelta = (Vector2)Input.mousePosition - (Vector2)lastPanPosition;
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        // Apply panning logic
        if (inputDelta != Vector2.zero)
        {
            float moveX = inputDelta.x * panSpeed * (invertX ? 1 : -1);
            float moveZ = inputDelta.y * panSpeed * (invertY ? 1 : -1);

            Vector3 panDelta = new Vector3(moveX, 0, moveZ);
            targetPosition += panDelta;

            // Clamp target position to bounds
            targetPosition.x = Mathf.Clamp(targetPosition.x, panLimitX.x, panLimitX.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, panLimitY.x, panLimitY.y);
        }

        // Smoothly move the camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void CenterTargetInView(Transform target)
    {
        Camera cam = Camera.main;
        if (cam == null || target == null)
        {
            Debug.LogWarning("Camera or target missing.");
            return;
        }

        // Target's current screen position
        Vector3 targetScreenPos = cam.WorldToScreenPoint(target.position);

        // Convert screenDelta to world delta at the target's depth
        Vector3 camPos = transform.position;

        Vector3 worldCenter = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, targetScreenPos.z));
        Vector3 worldTarget = cam.ScreenToWorldPoint(new Vector3(targetScreenPos.x, targetScreenPos.y + 25, targetScreenPos.z));

        Vector3 worldDelta = worldTarget - worldCenter;

        // Move camera opposite to worldDelta on X and Y, keep Z constant
        Vector3 desiredCamPos = camPos + new Vector3(worldDelta.x, 0, worldDelta.z);

        // Clamp within pan limits on X and Y
        desiredCamPos.x = Mathf.Clamp(desiredCamPos.x, panLimitX.x, panLimitX.y);
        desiredCamPos.z = Mathf.Clamp(desiredCamPos.z, panLimitY.x, panLimitY.y);

        // Keep Z as is
        desiredCamPos.y = camPos.y;

        targetPosition = desiredCamPos;
    }

}
