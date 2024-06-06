using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private RectTransform joystickRect;
    [SerializeField] private float flickSensitivity = 0.2f;

    public event UnityAction<Vector3> OnJoystickStart;
    public event UnityAction<Vector3> OnJoystickUpdate;
    public event UnityAction OnJoystickEnd;
    public event UnityAction<Vector3> OnJoystickFlicked;

    private const float JOYSTICK_LENGTH = 250f;

    private Vector3 defaultPosition;
    private Vector3 startPosition;

    public bool IsActive { get; private set; }

    private void Start()
    {
        defaultPosition = rect.position;
    }


    private float timer;
    private bool flicked;

    private void Update()
    {
        if (IsActive && Input.GetMouseButtonUp(0))
        {
            IsActive = false;
            rect.position = defaultPosition;
            joystickRect.localPosition = Vector3.zero;
            OnJoystickEnd?.Invoke();
        }

        if (IsPointerOverUI) return;

        if (Input.GetMouseButtonDown(0))
        {
            IsActive = true;
            startPosition = Input.mousePosition;
            rect.position = startPosition;
            OnJoystickStart?.Invoke(startPosition);
            timer = 0;
            flicked = false;
        }

        timer += Time.deltaTime;

        if (IsActive && !IsPointerOverUI && Input.GetMouseButton(0))
        {
            var direction = Vector3.ClampMagnitude(Input.mousePosition - startPosition, JOYSTICK_LENGTH);
            joystickRect.position = rect.position + direction;
            OnJoystickUpdate?.Invoke(direction);

            if (!flicked && timer < flickSensitivity && direction.magnitude == JOYSTICK_LENGTH)
            {
                flicked = true;
                OnJoystickFlicked?.Invoke(direction);
            }
        }
    }

    public bool IsPointerOverUI
    {
        get
        {
            var currentGameObject = EventSystem.current.currentSelectedGameObject;
            return currentGameObject && currentGameObject != gameObject;
        }
           
    }
}