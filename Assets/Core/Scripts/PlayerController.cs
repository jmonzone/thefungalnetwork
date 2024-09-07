using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MoveController movementController;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private List<Transform> cameraAnchors;


    private Transform virtualCameraAnchor;
    private Animator animator;
    private FungalController talkingFungal;

    private void Awake()
    {
        //movementController = GetComponent<MoveController>();
        //Movement.OnStart += () => animator.SetBool("isMoving", true);
        //Movement.OnEnd += () => animator.SetBool("isMoving", false);
        //Movement.OnUpdate += direction => animator.speed = direction.magnitude / 1.5f;

        animator = GetComponentInChildren<Animator>();
        //virtualJoystick.OnJoystickStart += _ => animator.SetBool("isMoving", true);
        virtualJoystick.OnJoystickEnd += () =>
        {
            //animator.SetBool("isMoving", false);
            movementController.Stop();
        };

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;
            movementController.SetDirection(direction);
        };
    }
    private void Update()
    {
        if (talkingFungal)
        {
            virtualCamera.transform.SetPositionAndRotation(virtualCameraAnchor.position, virtualCameraAnchor.rotation);
        }
    }

    public void SetMovementController(MoveController movement)
    {
        movementController.gameObject.SetActive(false);
        movementController = movement;
        cameraController.Target = movement.transform;
    }

    public void EndTalk()
    {
        if (talkingFungal)
        {
            talkingFungal = null;
            virtualCamera.Priority = 0;
        }
    }
}
