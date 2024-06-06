using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float cameraSpeed = 3f;
    [SerializeField] private Transform model;

    private Camera mainCamera;
    private Vector3 cameraOffset;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        var animator = GetComponentInChildren<Animator>();
        virtualJoystick.OnJoystickStart += _ => animator.SetBool("IsMoving", true);
        virtualJoystick.OnJoystickEnd += () =>
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("Speed", 0);
        };

        Debug.Log(name);

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            transform.position += 2.5f * playerSpeed * Time.deltaTime * direction.normalized;
            if(direction.magnitude > 0) transform.forward = direction;
            animator.SetFloat("Speed", direction.magnitude);

            model.localPosition = Vector3.zero;
        };

        rigidbody = GetComponent<Rigidbody>();
        virtualJoystick.OnJoystickFlicked += direction =>
        {
            Debug.Log("flicked");
            rigidbody.AddForce(direction.normalized * 100f);
            animator.SetTrigger("Dash");

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
