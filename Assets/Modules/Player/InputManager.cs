using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;

    public MovementController Movement => movementController;

    public event UnityAction<MovementController> OnMovementChanged;

    private void Awake()
    {
        //virtualJoystick.OnJoystickStart += _ => movementController.StartMovement();
        virtualJoystick.OnJoystickEnd += () => movementController.Stop();

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;
            movementController.SetDirection(direction);
        };
    }

    public void SetMovementController(MovementController movement)
    {
        movement.Stop();
        movementController = movement;
        cameraController.Target = movement.transform;
        OnMovementChanged?.Invoke(movement);
    }
}
