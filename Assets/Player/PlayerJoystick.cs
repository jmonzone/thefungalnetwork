using UnityEngine;

public class PlayerJoystick : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private InteractionController interaction;
    [SerializeField] private PhotoReference photoReference;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        virtualJoystick.OnJoystickStart += VirtualJoystick_OnJoystickStart;
        virtualJoystick.OnJoystickUpdate += VirtualJoystick_OnJoystickUpdate;
        virtualJoystick.OnJoystickEnd += VirtualJoystick_OnJoystickEnd; ;
    }

    private void VirtualJoystick_OnJoystickEnd()
    {
        playerReference.SetTargetPosition(playerReference.TargetPosition);
    }

    private void VirtualJoystick_OnJoystickStart(Vector3 arg0)
    {
        interaction.Unselect();
    }

    private void VirtualJoystick_OnJoystickUpdate(Vector3 direction)
    {
        // Map joystick to XZ plane
        direction.z = direction.y;
        direction.y = 0;

        // Get camera forward/right on XZ plane
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Rotate joystick input to be relative to camera
        Vector3 moveDir = camForward * direction.z + camRight * direction.x;

        // Compute target position
        var targetPosition = playerReference.Player.transform.position + moveDir.normalized;

        // Move player
        playerReference.SetTargetPosition(targetPosition);

        // Keep camera centered
        cameraPanController.CenterTargetInView(targetPosition);
    }

}
