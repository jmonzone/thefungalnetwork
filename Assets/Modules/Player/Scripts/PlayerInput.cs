using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    //[SerializeField] private Button interactionButton;
    [SerializeField] private Button primaryButton;
    [SerializeField] private Controller controller;
    [SerializeField] private Volume volume;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        primaryButton.onClick.AddListener(() =>
        {
            if (controller.Interactions.TargetAction) controller.Interactions.TargetAction.Use();
            else controller.Movement.Jump();
        });

        virtualJoystick.OnJoystickEnd += () =>
        {
            if (controller == null) return;
            controller.Movement.Stop();
        };

        virtualJoystick.OnJoystickUpdate += input =>
        {
            if (controller == null) return;
            var direction = new Vector3(input.x, 0, input.y);
            ApplyDirection(direction);
        };

        //todo: have this initialized in GameManager
        controller.Initialize(volume);
       
    }

    private void OnEnable()
    {
        controller.OnUpdate += UpdateCameraController;
    }

    private void OnDisable()
    {
        controller.OnUpdate -= UpdateCameraController;
    }

    private void UpdateCameraController()
    {
        cameraController.Target = controller.Movement.transform;
    }

    public void CanInteract(bool value)
    {
        primaryButton.interactable = value;
    }


    private ProximityAction previousAction;
    private bool usingWASD;

    private void Update()
    {
        UpdateWASDMovment();
        UpdateProximityActions();

        if (controller.Movement == null) return;
        var interaction = controller.Interactions && controller.Interactions.TargetAction && controller.Interactions.TargetAction.Interactable;
        CanInteract(interaction || controller.Movement.CanJump);
    }

    private void UpdateWASDMovment()
    {
        if (controller.Movement == null) return;

        // Read WASD or Arrow Key input
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow

        // Create movement direction based on input
        var direction = new Vector3(horizontal, 0, vertical).normalized;

        if (usingWASD)
        {
            // Stop movement if no input is detected
            if (direction == Vector3.zero)
            {
                controller.Movement.Stop();
                usingWASD = false;
                return;
            }
            else
            {
                ApplyDirection(direction);
            }
        }
        else if (direction != Vector3.zero)
        {
            usingWASD = true;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.Movement.Jump();
        }
    }

    private void ApplyDirection(Vector3 direction)
    {
        // Adjust direction relative to the camera's rotation
        direction = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * direction;

        // Set the movement direction
        controller.Movement.SetDirection(direction);
    }

    private void UpdateProximityActions()
    {
        if (!controller.Interactions) return;

        var targetAction = controller.Interactions.TargetAction;

        if (previousAction && previousAction != targetAction) previousAction.SetInRange(false);
        previousAction = targetAction;

        if (targetAction) targetAction.SetInRange(true);
    }
}
