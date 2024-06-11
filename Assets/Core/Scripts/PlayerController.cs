using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private List<Transform> cameraAnchors;

    private FungalController fungal;
    private Transform virtualCameraAnchor;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        virtualJoystick.OnJoystickStart += _ => animator.SetBool("isMoving", true);
        virtualJoystick.OnJoystickEnd += () => animator.SetBool("isMoving", false);

        virtualJoystick.OnJoystickUpdate += input =>
        {
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * direction;

            MoveInDirection(direction * 0.01f);
        };
    }

    private void Update()
    {
        if (fungal)
        {
            virtualCamera.transform.SetPositionAndRotation(virtualCameraAnchor.position, virtualCameraAnchor.rotation);
        }
    }

    public void MoveToTarget(Transform target, UnityAction onComplete)
    {
        StartCoroutine(MoveRoutine(target, onComplete));
    }

    public void LookAtTarget(Transform target)
    {
        var direction = target.position - transform.position;
        direction.y = 0;
        transform.forward = direction;
    }

    private IEnumerator MoveRoutine(Transform target, UnityAction onComplete)
    {
        animator.SetBool("isMoving", true);

        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            var direction = target.position - transform.position;
            direction.y = 0;
            MoveInDirection(direction.normalized * 2f);
            yield return null;
        }

        transform.position = target.position;

        animator.SetBool("isMoving", false);

        onComplete?.Invoke();
    }

    private void MoveInDirection(Vector3 direction)
    {
        animator.speed = direction.magnitude / 2.5f;
        transform.position += playerSpeed * Time.deltaTime * direction;
        if (direction.magnitude > 0) transform.forward = direction;
    }

    public void TalkToFungal(FungalController fungal)
    {
        this.fungal = fungal;
        fungal.SetTarget(transform);

        LookAtTarget(fungal.transform);

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
