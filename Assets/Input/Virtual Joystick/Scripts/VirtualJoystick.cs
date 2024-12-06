using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private RectTransform joystickRect;
    [SerializeField] private GameObject target;
    [SerializeField] private float sensitivity;

    public event UnityAction<Vector3> OnJoystickStart;
    public event UnityAction<Vector3> OnJoystickUpdate;
    public event UnityAction OnJoystickEnd;

    private const float JOYSTICK_LENGTH = 100f;

    private Vector3 defaultPosition;
    private Vector3 startPosition;

    public bool IsActive { get; private set; }

    private void Start()
    {
        defaultPosition = rect.anchoredPosition;
    }

    private int activeTouchIndex = -1; // Tracks the touch index (-1 for mouse)
    private bool IsTouch => activeTouchIndex >= 0;

    private void Update()
    {
        // Handle joystick release
        if (IsActive && IsInputReleased())
        {
            IsActive = false;
            activeTouchIndex = -1;
            rect.anchoredPosition = defaultPosition;
            joystickRect.anchoredPosition = Vector3.zero;
            OnJoystickEnd?.Invoke();
        }

        // Handle joystick start
        if (!IsActive && IsPointerOverTarget && IsInputPressed(out int touchIndex))
        {
            IsActive = true;
            activeTouchIndex = touchIndex;
            startPosition = GetInputPosition();
            rect.position = startPosition;
            OnJoystickStart?.Invoke(startPosition);
        }

        // Handle joystick movement
        if (IsActive && IsInputHeld())
        {
            var direction = Vector3.ClampMagnitude(GetInputPosition() - startPosition, JOYSTICK_LENGTH);
            joystickRect.position = rect.position + direction;
            OnJoystickUpdate?.Invoke(direction * sensitivity);
        }
    }

    // Get input position for the active touch or mouse
    private Vector3 GetInputPosition()
    {
        if (IsTouch && activeTouchIndex < Input.touchCount)
            return Input.GetTouch(activeTouchIndex).position;
        return Input.mousePosition;
    }

    // Check if any input has been pressed, return touch index or -1 for mouse
    private bool IsInputPressed(out int touchIndex)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                touchIndex = i;
                return true;
            }
        }

        touchIndex = -1;
        return Input.GetMouseButtonDown(0);
    }

    // Check if the active input is being held
    private bool IsInputHeld()
    {
        if (IsTouch && activeTouchIndex < Input.touchCount)
        {
            var phase = Input.GetTouch(activeTouchIndex).phase;
            return phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
        }
        return Input.GetMouseButton(0);
    }

    // Check if the active input has been released
    private bool IsInputReleased()
    {
        if (IsTouch && activeTouchIndex < Input.touchCount)
        {
            return Input.GetTouch(activeTouchIndex).phase == TouchPhase.Ended;
        }
        return Input.GetMouseButtonUp(0);
    }

    public bool IsPointerOverTarget
    {
        get
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = GetInputPosition()
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            return !target || (results.Count > 0 && results[0].gameObject == target);
        }
    }

}