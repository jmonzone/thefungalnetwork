using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private LookController lookController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private List<Transform> cameraAnchors;

    private FungalController fungal;
    private Transform virtualCameraAnchor;

    private void Awake()
    {
        var animator = GetComponentInChildren<Animator>();
        virtualJoystick.OnJoystickStart += _ => animator.SetBool("isMoving", true);
        virtualJoystick.OnJoystickEnd += () => animator.SetBool("isMoving", false);

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            animator.speed = direction.magnitude / 250f;
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;

            transform.position += playerSpeed * 0.01f * Time.deltaTime * direction;
            if(direction.magnitude > 0) transform.forward = direction;
        };
    }

    public void TalkToFungal(FungalController fungal)
    {
        this.fungal = fungal;
        fungal.SetTarget(transform);

        var direction = fungal.transform.position - transform.position;
        direction.y = 0;
        transform.forward = direction;
        //lookController.Direction = direction;
        //lookController.enabled = true;

        virtualCamera.Priority = 2;

        var mainCamera = Camera.main.transform;
        var closestDistance = Mathf.Infinity;
        foreach(var cameraAnchor in cameraAnchors)
        {
            var distance = Vector3.Distance(mainCamera.position, cameraAnchor.position);

            if (distance > closestDistance) continue;

            virtualCameraAnchor = cameraAnchor;
            closestDistance = distance;
        }
    }

    private void Update()
    {
        if (fungal)
        {
            virtualCamera.transform.SetPositionAndRotation(virtualCameraAnchor.position, virtualCameraAnchor.rotation);
        }
    }

    public void EndTalk()
    {
        if (fungal)
        {
            lookController.enabled = false;
            if (!fungal.IsFollowing) fungal.SetTarget(null);
            fungal = null;
            virtualCamera.Priority = 0;
        }
    }

    //private void 

    //private void SetLookDirection(Vector3 direction)
    //{
    //    if (faceForward) Forward = Vector3.Lerp(Forward, direction, 5f * Time.deltaTime);

    //}
}
