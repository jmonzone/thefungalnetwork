using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MoveController movementController;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;

    private void Awake()
    {
        virtualJoystick.OnJoystickStart += _ => movementController.StartMovement();
        virtualJoystick.OnJoystickEnd += () => movementController.Stop();

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;
            movementController.SetDirection(direction);
        };
    }

    private void Start()
    {
        FungalManager.Instance.OnInteractionStarted += OnFungalInteractionStarted;
    }

    private void OnFungalInteractionStarted(FungalController fungal)
    {
        SetMovementController(fungal.Movement);
    }

    private void SetMovementController(MoveController movement)
    {
        movementController = movement;
        cameraController.Target = movement.transform;
    }
}
