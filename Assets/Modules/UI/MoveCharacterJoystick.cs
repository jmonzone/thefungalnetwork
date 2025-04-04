using UnityEngine;

public class MoveCharacterJoystick : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private GameReference game;
    [SerializeField] private float wasdSensitivity;

    private void Awake()
    {
        joystick.OnJoystickUpdate += MovePlayer;
        joystick.OnJoystickEnd += Joystick_OnJoystickEnd;
    }

    private void Joystick_OnJoystickEnd()
    {
        if (!enabled) return;
        //Debug.Log("Joystick_OnJoystickEnd");
        game.ClientPlayer.Fungal.Movement.Stop();
    }

    private void Update()
    {
        if (!game.ClientPlayer.Fungal) return;

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
        if (game.ClientPlayer.Fungal.IsDead) return;

        var translation = direction;
        translation.z = translation.y;
        translation.y = 0;

        translation = Quaternion.Euler(0, transform.eulerAngles.y, 0) * translation;

        game.ClientPlayer.Fungal.Movement.SetDirection(translation);
    }
}
