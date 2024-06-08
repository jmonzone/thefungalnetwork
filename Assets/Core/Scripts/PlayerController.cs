using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private Transform cameraController;


    // Start is called before the first frame update
    void Start()
    {
        var animator = GetComponentInChildren<Animator>();
        virtualJoystick.OnJoystickStart += _ => animator.SetBool("isMoving", true);
        virtualJoystick.OnJoystickEnd += () => animator.SetBool("isMoving", false);

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            animator.speed = direction.magnitude / 250f;
            direction = Quaternion.Euler(0, cameraController.eulerAngles.y, 0) * direction;

            transform.position += playerSpeed * 0.01f * Time.deltaTime * direction;
            if(direction.magnitude > 0) transform.forward = direction;
        };
    }
}
