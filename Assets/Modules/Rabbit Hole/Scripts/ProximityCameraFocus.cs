using Cinemachine;
using UnityEngine;

public class ProximityCameraFocus : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        virtualCamera.Priority = 2;
    }

    private void OnTriggerExit(Collider other)
    {
        virtualCamera.Priority = 0;
    }
}
