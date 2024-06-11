using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private List<Transform> cameraAnchors;

    private FungalController fungal;
    private Transform virtualCameraAnchor;
    private Animator animator;

    public MoveController Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<MoveController>();
        Movement.OnStart += () => animator.SetBool("isMoving", true);
        Movement.OnEnd += () => animator.SetBool("isMoving", false);
        Movement.OnUpdate += () => animator.speed = Movement.Direction.magnitude / 1.5f;

        animator = GetComponentInChildren<Animator>();
        virtualJoystick.OnJoystickStart += _ => animator.SetBool("isMoving", true);
        virtualJoystick.OnJoystickEnd += () => animator.SetBool("isMoving", false);

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;

            Movement.MoveInDirection(direction);
        };
    }

    private void Update()
    {
        if (fungal)
        {
            virtualCamera.transform.SetPositionAndRotation(virtualCameraAnchor.position, virtualCameraAnchor.rotation);
        }
    }

    public void TalkToFungal(FungalController fungal)
    {
        this.fungal = fungal;
        fungal.SetTarget(transform);

        Movement.LookAtTarget(fungal.transform);

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

    public void EndTalk()
    {
        if (fungal)
        {
            if (!fungal.IsFollowing) fungal.SetTarget(null);
            fungal = null;
            virtualCamera.Priority = 0;
        }
    }
}
