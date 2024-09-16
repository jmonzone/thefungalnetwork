using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private MoveController movementController;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;

    public MoveController Movement => movementController;

    private void Awake()
    {
        Instance = this;

        //virtualJoystick.OnJoystickStart += _ => movementController.StartMovement();
        virtualJoystick.OnJoystickEnd += () => movementController.Stop();

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;
            movementController.SetDirection(direction);
        };
    }

    public void SetMovementController(MoveController movement)
    {
        movementController = movement;
        cameraController.Target = movement.transform;
    }
}
