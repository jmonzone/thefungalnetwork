using UnityEngine;

public class CameraPanController : MonoBehaviour
{
    public float panSpeed = 0.1f;
    public float smoothTime = 0.2f;
    public bool invertX = false;
    public bool invertZ = false;

    public Vector2 panLimitX = new Vector2(-5f, 5f);  // left/right bounds
    public Vector2 panLimitZ = new Vector2(-5f, 5f);  // forward/back bounds

    private Vector3 lastPanPosition;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private bool isPanning;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        Vector2 inputDelta = Vector2.zero;

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
            float moveZ = inputDelta.y * panSpeed * (invertZ ? 1 : -1);

            Vector3 panDelta = new Vector3(moveX, moveZ);
            targetPosition += panDelta;

            // Clamp target position to bounds
            targetPosition.x = Mathf.Clamp(targetPosition.x, panLimitX.x, panLimitX.y);
            targetPosition.y = Mathf.Clamp(targetPosition.y, panLimitZ.x, panLimitZ.y);
        }

        // Smoothly move the camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
