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
    [SerializeField] private float zoomSmoothTime = 0.2f;
    [SerializeField] private float zoomMin = 2f;
    [SerializeField] private float zoomMax = 15f;

    private bool isPanning;
    private Vector3 lastPanPosition;
    private Vector3 targetPosition;
    private Vector3 panVelocity = Vector3.zero;

    private float targetOrthoSize;
    private float zoomVelocity = 0f; // store velocity for SmoothDamp

    public Vector2 inputDelta;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        targetPosition = transform.position;
        targetOrthoSize = virtualCamera.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        // Smooth panning
        virtualCamera.transform.position = Vector3.SmoothDamp(virtualCamera.transform.position, targetPosition, ref panVelocity, panSmoothTime);

        // Smoothly interpolate to target size
        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
            virtualCamera.m_Lens.OrthographicSize,
            targetOrthoSize,
            ref zoomVelocity,
            zoomSmoothTime // smoothing time in seconds
        );

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        inputDelta = Vector2.zero;


        // Mouse scroll / trackpad pinch
        float scroll = Input.GetAxis("Mouse ScrollWheel");

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
            ZoomCamera(delta * zoomSpeed * 0.1f * Time.deltaTime);
        }

        else if (Mathf.Abs(scroll) > 0.001f)
        {
            ZoomCamera(scroll * zoomSpeed);
        }

        // Single touch / mouse panning
        else HandlePanning();
    }

    private void HandlePanning()
    {
        Debug.Log("handling panning");

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
        targetOrthoSize = Mathf.Clamp(targetOrthoSize - increment, zoomMin, zoomMax);
    }

    public void CenterTargetInView(Vector3 position)
    {
        //Debug.Log("Centering");
        panSmoothTime = centeringSmoothTime;

        // Target's current screen position
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(position);

        // Convert screenDelta to world delta at the target's depth
        Vector3 camPos = transform.position;

        Vector3 worldCenter = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, targetScreenPos.z));
        Vector3 worldTarget = mainCamera.ScreenToWorldPoint(new Vector3(targetScreenPos.x, targetScreenPos.y, targetScreenPos.z));

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
