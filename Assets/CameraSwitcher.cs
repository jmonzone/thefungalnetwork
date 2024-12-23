using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private static CinemachineVirtualCamera currentCamera;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        var movement = other.GetComponentInParent<MovementController>();
        if (movement && movement == controller.Movement)
        {
            if (currentCamera && currentCamera != virtualCamera) currentCamera.Priority = 0;
            currentCamera = virtualCamera;
            currentCamera.Priority = 2;
        }
    }
}
