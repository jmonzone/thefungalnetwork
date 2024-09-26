using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // The target to rotate around
    [SerializeField] private float rotationSpeed = 0.1f; // Adjust the rotation speed
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float minVerticalAngle = -30f; // Minimum vertical angle
    [SerializeField] private float maxVerticalAngle = 60f; // Maximum vertical angle

    [SerializeField] private bool rotateCamera = true;

    private Vector2 lastInputPosition;
    private bool isDragging = false;
    private Vector3 cameraOffset;
    private float currentVerticalAngle = 0f;

    public Transform Target { get => target; set => target = value; }

    private void Update()
    {
        if (rotateCamera) HandleRotateCamera();

        if (!target) return;

        var targetPosition = target.position;
        var direction = targetPosition - transform.position;

        if (direction.magnitude > 0.05f)
        {
            transform.position += movementSpeed * Time.deltaTime * direction;
        }

    }

    private void HandleRotateCamera()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Debug.Log("touching");
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsPointerOverUI) return;
                    lastInputPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 deltaTouch = touch.position - lastInputPosition;
                        lastInputPosition = touch.position;

                        RotateCamera(deltaTouch);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
        // Check for mouse input in the editor
        else if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI) return;
            lastInputPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 deltaMouse = (Vector2)Input.mousePosition - lastInputPosition;
            lastInputPosition = Input.mousePosition;

            RotateCamera(deltaMouse);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void RotateCamera(Vector2 delta)
    {
        float angleX = delta.x * rotationSpeed;
        float angleY = delta.y * rotationSpeed;

        var targetPoint = target != null ? target.position : Vector3.zero;

        // Rotate the camera around the target on the Y-axis for horizontal swipe
        transform.RotateAround(targetPoint, Vector3.up, angleX);

        // Calculate the new vertical angle and clamp it
        float newVerticalAngle = Mathf.Clamp(currentVerticalAngle - angleY, minVerticalAngle, maxVerticalAngle);

        // Apply the clamped vertical rotation
        float verticalRotation = newVerticalAngle - currentVerticalAngle;
        transform.RotateAround(targetPoint, transform.right, verticalRotation);

        // Update the current vertical angle
        currentVerticalAngle = newVerticalAngle;
    }

    private bool IsPointerOverUI
    {
        get
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                // Set the pointer position
                position = Input.mousePosition
            };

            // Raycast to all UI elements under the pointer position
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
           
            return results.Count > 0;
        }
    }
}
