using UnityEngine;

public class MultiplayerTest : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;

    private Transform player;

    private void Start()
    {
        virtualJoystick.OnJoystickUpdate += direction =>
        {
            if (!player) return;
            var mappedDirection = new Vector3(direction.x, 0, direction.y);
            player.transform.position += mappedDirection;
        };

        NetworkPlayer.OnLocalPlayerSpawned += player =>
        {
            this.player = player;
            cameraController.Target = player;
        };
    }
}
