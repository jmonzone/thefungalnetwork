using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private Transform cameraController;

    private void Awake()
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

    //private void 

    //private void SetLookDirection(Vector3 direction)
    //{
    //    if (faceForward) Forward = Vector3.Lerp(Forward, direction, 5f * Time.deltaTime);

    //}
}
