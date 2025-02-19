using UnityEngine;

public class MoveCharacterJoystick : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private PlayerReference player;
    [SerializeField] private float wasdSensitivity;
    [SerializeField] private float speed = 2f;

    private void Awake()
    {
        joystick.OnJoystickUpdate += MoveReticle;
    }

    private void Update()
    {
        if (!player.Movement) return;

        if (Application.isEditor)
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            var direction = wasdSensitivity * Time.deltaTime * new Vector3(x, y);
            MoveReticle(direction);
        }
    }

    private void MoveReticle(Vector3 direction)
    {
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
        player.Movement.transform.forward = translation;
    }
}
