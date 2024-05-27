using UnityEngine;

public class GardenManager : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private Transform player;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float cameraSpeed = 3f;

    private Camera mainCamera;
    private Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            player.transform.position += playerSpeed * 0.01f * Time.deltaTime * direction;
        };

        mainCamera = Camera.main;
        cameraOffset = mainCamera.transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var targetPosition = player.transform.position + cameraOffset;
        var direction = targetPosition - mainCamera.transform.position;

        if (direction.magnitude > 0.05f)
        {
            mainCamera.transform.position += cameraSpeed * Time.deltaTime * direction;
        }
    }
}
