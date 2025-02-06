using UnityEngine;

public class FishingJoystick : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Transform reticle;

    private void Awake()
    {
        joystick.OnJoystickUpdate += MoveReticle;
        reticle.gameObject.SetActive(true);
    }

    private void Update()
    {
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

        reticle.transform.position += translation;
    }
}
