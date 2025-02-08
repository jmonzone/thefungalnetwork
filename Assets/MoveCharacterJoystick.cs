using UnityEngine;

public class MoveCharacterJoystick : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Controller controller;

    private void Awake()
    {
        joystick.OnJoystickUpdate += MoveReticle;
    }

    private void Update()
    {
        if (!controller.Transform) return;

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var direction = new Vector3(x, y) * Time.deltaTime;
        MoveReticle(direction);
    }

    private void MoveReticle(Vector3 direction)
    {
        var translation = direction;
        translation.z = direction.y;
        translation.y = 0;

        translation = Quaternion.Euler(0, transform.eulerAngles.y, 0) * translation;

        if (translation == Vector3.zero) return;

        controller.Transform.position += translation;
        controller.Transform.forward = translation;
    }
}
