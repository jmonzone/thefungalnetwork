using UnityEngine;

public class PlayerJoystick : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private VirtualJoystick virtualJoystick;

    private void Awake()
    {
        virtualJoystick.OnJoystickUpdate += VirtualJoystick_OnJoystickUpdate;
    }

    private void VirtualJoystick_OnJoystickUpdate(Vector3 direction)
    {
        direction.z = direction.y;
        direction.y = 0;

        var targetPosition = playerReference.Player.transform.position + direction.normalized;
        playerReference.SetTargetPosition(targetPosition);

        cameraPanController.CenterTargetInView(targetPosition);
    }
}
