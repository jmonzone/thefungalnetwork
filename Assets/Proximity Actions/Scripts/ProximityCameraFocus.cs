using Cinemachine;
using UnityEngine;

public class ProximityCameraFocus : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Controllable>() == controller.Controllable)
        {
            virtualCamera.Priority = 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Controllable>() == controller.Controllable)
        {
            virtualCamera.Priority = 0;
        }
    }
}
