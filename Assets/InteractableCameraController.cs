using Cinemachine;
using UnityEngine;

public class InteractableCameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void ActivateCamera()
    {
        virtualCamera.Priority = 100;
    }

    public void DeactivateCamera()
    {
        virtualCamera.Priority = 0;
    }
}
