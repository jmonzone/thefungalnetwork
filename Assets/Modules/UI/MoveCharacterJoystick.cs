using UnityEngine;

public class MoveCharacterJoystick : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private float wasdSensitivity;

    public Movement player;

    private void Start()
    {
        //Debug.Log($"MoveCharacterJoystick.Start {player.name}");

        joystick.OnJoystickUpdate += MovePlayer;
        joystick.OnJoystickEnd += Joystick_OnJoystickEnd;
    }

    private void Joystick_OnJoystickEnd()
    {
        if (!enabled) return;
        //Debug.Log("Joystick_OnJoystickEnd");
        player.Stop();
    }

    private void Update()
    {
        if (!player) return;

        if (Application.isEditor && !joystick.IsActive)
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            var direction = wasdSensitivity * new Vector3(x, y);

            MovePlayer(direction);
        }
    }

    private void MovePlayer(Vector3 direction)
    {
        if (!enabled) return;
        if (!player) return;

        var translation = direction;
        translation.z = translation.y;
        translation.y = 0;

        translation = Quaternion.Euler(0, transform.eulerAngles.y, 0) * translation;

        player.SetDirection(translation);
    }
}
