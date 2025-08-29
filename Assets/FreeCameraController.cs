using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 0.2f;
    public float pinchZoomSpeed = 0.1f;

    [Header("Smoothing Settings")]
    public float rotationSmoothTime = 0.1f;
    public float movementSmoothTime = 0.1f;

    [Header("Movement Bounds")]
    public Transform localSpace;
    public Bounds movementBounds;
    public LayerMask groundMask;
    public VirtualJoystick virtualJoystick;

    [Header("Camera Clamp")]
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public float fixedYHeight = 5f;

    private Vector2 orbitAngles = Vector2.zero; // target rotation
    private Vector2 currentRotation = Vector2.zero;
    private Vector2 rotationVelocity = Vector2.zero;

    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        targetPosition = transform.position;
        orbitAngles = transform.eulerAngles;
        virtualJoystick.OnJoystickUpdate += VirtualJoystick_OnJoystickUpdate;
    }

    private void VirtualJoystick_OnJoystickUpdate(Vector3 input)
    {
        // Map input to local forward/right
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        // Joystick: input.x = horizontal, input.y = vertical
        Vector3 move = forward * input.y + right * input.x;

        // Apply movement speed
        move *= moveSpeed * Time.deltaTime;

        targetPosition += move;

        // Clamp within bounds
        targetPosition = ClampToBounds(targetPosition);
    }

    void Update()
    {
        HandleTouchInput();
        SmoothApply();
    }

    void HandleTouchInput()
    {
        // --- One-finger drag for rotation ---
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            // Ignore if over UI
            if (virtualJoystick.IsActive) return;

            if (t.phase == TouchPhase.Moved)
            {
                orbitAngles.y += -t.deltaPosition.x * rotationSpeed;
                orbitAngles.x = Mathf.Clamp(orbitAngles.x + t.deltaPosition.y * rotationSpeed, minPitch, maxPitch);
            }

            // Tap to move
            //if (t.phase == TouchPhase.Ended && t.tapCount == 1)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(t.position);
            //    if (Physics.Raycast(ray, out RaycastHit hit))
            //    {
            //        Vector3 newPos = hit.point;
            //        newPos.y = fixedYHeight;

            //        targetPosition = ClampToBounds(newPos);
            //    }
            //}
        }

        // --- Two-finger pinch to move forward/back (inverse) ---
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Ignore if either finger is over UI
            //if ((EventSystem.current != null &&
            //     (EventSystem.current.IsPointerOverGameObject(t0.fingerId) ||
            //      EventSystem.current.IsPointerOverGameObject(t1.fingerId))))
            //    return;

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDistance = Vector2.Distance(prev0, prev1);
            float currDistance = Vector2.Distance(t0.position, t1.position);

            // Inverse: spread fingers → move forward, pinch fingers → move back
            float delta = (currDistance - prevDistance) * pinchZoomSpeed;

            Vector3 move = transform.forward * delta;
            move.y = 0f;
            targetPosition += move;

            targetPosition = ClampToBounds(targetPosition);
        }

        // --- Editor support ---
        if (Application.isEditor)
        {
            // Rotation with mouse drag
            if (Input.GetMouseButton(0))
            {
                Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * rotationSpeed * 100f;
                orbitAngles.y += delta.x;
                orbitAngles.x = Mathf.Clamp(orbitAngles.x - delta.y, minPitch, maxPitch);
            }

            var wasdInput = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) wasdInput += Vector3.forward;
            if (Input.GetKey(KeyCode.A)) wasdInput += Vector3.left;
            if (Input.GetKey(KeyCode.S)) wasdInput += Vector3.back;
            if (Input.GetKey(KeyCode.D)) wasdInput += Vector3.right;

            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            // Joystick: input.x = horizontal, input.y = vertical
            Vector3 move = forward * wasdInput.z + right * wasdInput.x;

            // Apply movement speed
            move *= moveSpeed * Time.deltaTime * 10f;

            move.y = 0f;
            targetPosition += move;

            targetPosition = ClampToBounds(targetPosition);

            //// Tap to move (right-click)
            //if (Input.GetMouseButtonDown(1))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundMask))
            //    {
            //        Vector3 newPos = hit.point;
            //        newPos.y = fixedYHeight;

            //        targetPosition = ClampToBounds(newPos);
            //    }
            //}
        }
    }

    Vector3 ClampToBounds(Vector3 worldPos)
    {
        // Convert world position to local space of the box
        //Vector3 localPos = localSpace.InverseTransformPoint(worldPos);
        Vector3 localPos = worldPos;

        Vector3 halfSize = movementBounds.size * 0.5f;
        localPos.x = Mathf.Clamp(localPos.x, -halfSize.x, halfSize.x);
        localPos.y = Mathf.Clamp(localPos.y, -halfSize.y, halfSize.y);
        localPos.z = Mathf.Clamp(localPos.z, -halfSize.z, halfSize.z);

        // Convert back to world space
        //return localSpace.TransformPoint(localPos);
        return localPos;
    }

    void SmoothApply()
    {
        // Smooth rotation
        currentRotation = Vector2.SmoothDamp(currentRotation, orbitAngles, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);

        // Smooth movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, movementSmoothTime);
    }
}
