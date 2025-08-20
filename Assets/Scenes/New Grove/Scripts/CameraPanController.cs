using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPanController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 0.1f;
    [SerializeField] private float panSmoothTime = 0.2f;
    [SerializeField] private Vector2 panLimitX = new Vector2(-5f, 5f);  // left/right bounds
    [SerializeField] private Vector2 panLimitY = new Vector2(-5f, 5f);  // forward/back bounds
    [SerializeField] private float centeringSmoothTime = 0.1f;


    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomMin = 2f;
    [SerializeField] private float zoomMax = 15f;

    private bool isPanning;
    private Vector3 lastPanPosition;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    public Vector2 inputDelta;

    private void Awake()
    {
        targetPosition = transform.position;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        inputDelta = Vector2.zero;

        // Single touch / mouse panning
        HandlePanning();

        // Two-finger pinch zoom
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevDist = (prevT0 - prevT1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float delta = currDist - prevDist;
            ZoomCamera(delta * zoomSpeed * Time.deltaTime);
        }

        // Mouse scroll / trackpad pinch
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            ZoomCamera(scroll * zoomSpeed);
        }

        // Smooth panning
        virtualCamera.transform.position = Vector3.SmoothDamp(virtualCamera.transform.position, targetPosition, ref velocity, panSmoothTime);
    }

    private void HandlePanning()
    {
        // Single touch
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
        // Mouse
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

        // Apply panning
        if (inputDelta != Vector2.zero)
        {
            float moveX = inputDelta.x * -panSpeed; // adjust pan sensitivity
            float moveZ = inputDelta.y * -panSpeed;
            targetPosition += new Vector3(moveX, 0, moveZ);
        }
    }

    private void ZoomCamera(float increment)
    {
        float orthoSize = virtualCamera.m_Lens.OrthographicSize - increment;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(orthoSize, zoomMin, zoomMax);
    }

    public void CenterTargetInView(Vector3 position)
    {
        panSmoothTime = centeringSmoothTime;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Camera or target missing.");
            return;
        }

        // Target's current screen position
        Vector3 targetScreenPos = cam.WorldToScreenPoint(position);

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
