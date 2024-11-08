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

    private void Update()
    {
        if (IsActive && Input.GetMouseButtonUp(0))
        {
            IsActive = false;
            rect.anchoredPosition = defaultPosition;
            joystickRect.anchoredPosition = Vector3.zero;
            OnJoystickEnd?.Invoke();
        }

        if (IsPointerOverTarget && Input.GetMouseButtonDown(0))
        {
            IsActive = true;
            startPosition = Input.mousePosition;
            rect.position = startPosition;
            OnJoystickStart?.Invoke(startPosition);
        }

        if (IsActive && Input.GetMouseButton(0))
        {
            var direction = Vector3.ClampMagnitude(Input.mousePosition - startPosition, JOYSTICK_LENGTH);
            joystickRect.position = rect.position + direction;
            OnJoystickUpdate?.Invoke(direction * sensitivity);
        }
    }

    public bool IsPointerOverTarget
    {
        get
        {
            // Create a new PointerEventData
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                // Set the pointer position
                position = Input.mousePosition
            };

            // Raycast to all UI elements under the pointer position
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            if (!target) return true;
            else if (results.Count > 0 && results[0].gameObject == target) return true;
            else return false;
        }
           
    }
}