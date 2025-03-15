using UnityEngine;

public class MoveCharacterJoystick : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private PlayerReference player;
    [SerializeField] private float wasdSensitivity;
    [SerializeField] private float speed = 2f;

    private void Awake()
    {
        joystick.OnJoystickUpdate += MovePlayer;
        joystick.OnJoystickEnd += Joystick_OnJoystickEnd;
    }

    private void Joystick_OnJoystickEnd()
    {
        if (!enabled) return;
        Debug.Log("Joystick_OnJoystickEnd");
        player.Movement.Stop();
    }

    private void Update()
    {
        if (!player.Movement) return;

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
        if (player.Fungal.IsDead) return;

        var translation = direction;
        translation.z = translation.y;
        translation.y = 0;

        translation = Quaternion.Euler(0, transform.eulerAngles.y, 0) * translation;

        if (translation == Vector3.zero)
        {
            player.Movement.Stop();
            return;
        }

        player.Movement.SetDirection(translation, speed);
    }
}
