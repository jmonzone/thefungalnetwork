using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float cameraSpeed = 3f;

    private Camera mainCamera;
    private Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        var animator = GetComponentInChildren<Animator>();
        virtualJoystick.OnJoystickStart += _ => animator.SetBool("isMoving", true);
        virtualJoystick.OnJoystickEnd += () => animator.SetBool("isMoving", false);

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            transform.position += playerSpeed * 0.01f * Time.deltaTime * direction;
            if(direction.magnitude > 0) transform.forward = direction;
        };

        mainCamera = Camera.main;
        cameraOffset = mainCamera.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var targetPosition = transform.position + cameraOffset;
        var direction = targetPosition - mainCamera.transform.position;

        if (direction.magnitude > 0.05f)
        {
            mainCamera.transform.position += cameraSpeed * Time.deltaTime * direction;
        }
    }
}
