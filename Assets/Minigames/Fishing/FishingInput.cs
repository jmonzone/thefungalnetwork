using UnityEngine;

public class FishingInput : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Transform reticle;

    private void Awake()
    {
        joystick.OnJoystickUpdate += Joystick_OnJoystickUpdate;
        reticle.gameObject.SetActive(true);
    }

    private void Joystick_OnJoystickUpdate(Vector3 direction)
    {
        var translation = direction;
        translation.z = direction.y;
        translation.y = 0;

        translation = Quaternion.Euler(0, transform.eulerAngles.y, 0) * translation;

        reticle.transform.position += translation;
    }
}
