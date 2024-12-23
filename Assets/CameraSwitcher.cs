using Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private bool lockOnTarget = false;

    private void OnTriggerEnter(Collider other)
    {
        var movement = other.GetComponentInParent<MovementController>();
        if (movement && movement == controller.Movement)
        {
            if (lockOnTarget) virtualCamera.LookAt = controller.Movement.transform;
            virtualCamera.Priority = 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        virtualCamera.Priority = 0;
    }
}
