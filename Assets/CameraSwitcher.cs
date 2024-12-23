using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private bool lockOnTarget = false;

    public event UnityAction OnSwitch;

    private void OnTriggerEnter(Collider other)
    {
        var movement = other.GetComponentInParent<MovementController>();
        if (movement && movement == controller.Movement)
        {
            if (lockOnTarget) virtualCamera.LookAt = controller.Movement.transform;
            virtualCamera.Priority = 2;
            OnSwitch?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        virtualCamera.Priority = 0;
    }
}
