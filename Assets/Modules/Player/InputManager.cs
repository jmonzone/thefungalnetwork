using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button interactionButton;

    public MovementController Movement => movementController;

    public event UnityAction<MovementController> OnMovementChanged;
    public event UnityAction OnInteractionButtonClicked;

    private void Awake()
    {
        interactionButton.onClick.AddListener(() => OnInteractionButtonClicked?.Invoke());

        if (movementController) SetMovementController(movementController);

        virtualJoystick.OnJoystickEnd += () =>
        {
            if (!movementController) return;
            movementController.Stop();
        };

        virtualJoystick.OnJoystickUpdate += input =>
        {
            if (!movementController) return;
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.GetChild(0).eulerAngles.y, 0) * direction;
            movementController.SetDirection(direction);
        };
    }

    public void SetMovementController(MovementController movement)
    {
        movementController = movement;
        movement.Stop();
        cameraController.Target = movement.transform;
        OnMovementChanged?.Invoke(movement);
    }

    public void CanInteract(bool value)
    {
        interactionButton.interactable = value;
    }
}
