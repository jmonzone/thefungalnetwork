using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerReference controller;

    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button interactionButton;

    private Camera mainCamera;

    private ProximityAction interactionAction;

    private void Awake()
    {
        mainCamera = Camera.main;

        jumpButton.onClick.AddListener(() =>
        {
            HandleJump();
        });

        interactionButton.onClick.AddListener(() =>
        {
            if (interactionAction) interactionAction.Use();
        });

        virtualJoystick.OnJoystickEnd += () =>
        {
            if (controller == null) return;
            //controller.Movement.Stop();
        };

        virtualJoystick.OnJoystickUpdate += input =>
        {
            if (controller == null) return;
            var direction = new Vector3(input.x, 0, input.y);
            ApplyDirection(direction);
        };
    }

    private bool usingWASD;

    private const float MAXIMUM_PROXIMITY_DISTANCE = 3f;

    private void Update()
    {
        UpdateWASDMovment();

        //if (controller.Movement == null) return;

        //var targetActions = Physics.OverlapSphere(controller.Movement.transform.position, MAXIMUM_PROXIMITY_DISTANCE)
        //   .Where(collider => !collider.isTrigger)
        //   .Select(collider => collider.GetComponentInParent<ProximityAction>())
        //   .Where(action => action && action.transform != controller.Movement.transform)
        //   .OrderBy(entity => Vector3.Distance(controller.Movement.transform.position, entity.transform.position))
        //   .ToList();

        //var interactionAction = targetActions.FirstOrDefault();

        //if (this.interactionAction && this.interactionAction != interactionAction) this.interactionAction.SetInRange(false);
        //this.interactionAction = interactionAction;
        //if (interactionAction) this.interactionAction.SetInRange(true);

        //interactionButton.gameObject.SetActive(interactionAction && interactionAction.Interactable);
    }

    private void UpdateWASDMovment()
    {
        //if (controller.Movement == null) return;

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
                //controller.Movement.Stop();
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
            HandleJump();
        }
    }

    private void HandleJump()
    {
        // if controlling a mount, then make the rider jump
        //if (controller.Mount)
        //{
        //    Debug.Log("has mount");
        //    controller.Mount.MountController.Movement.Jump();
        //}
        //else
        //{
        //    Debug.Log("no mount");
        //    controller.Movement.Jump();
        //}
    }

    private void ApplyDirection(Vector3 direction)
    {
        // Adjust direction relative to the camera's rotation
        direction = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * direction;

        //todo: attackable should not be referenced , it should be can move
        //if (!controller.Attackable || controller.Attackable.CurrentHealth > 0)
        //{
        //    // Set the movement direction
        //    controller.Movement.SetDirection(direction);
        //}
    }
}
