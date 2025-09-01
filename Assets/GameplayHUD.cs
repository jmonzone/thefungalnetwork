using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference cameraView;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private CinemachineVirtualCamera gameplayVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera photoVirtualCamera;
    [SerializeField] private InteractionController interactionController;

    [Header("UI References")]
    [SerializeField] private Button cameraButton;
    [SerializeField] private Button backButton;
    [SerializeField] private VirtualJoystick virtualJoystick;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (cameraButton) cameraButton.onClick.AddListener(UseOrthographicCamera);
        if (backButton) backButton.onClick.AddListener(UsePerspectiveCamera);

        virtualJoystick.OnJoystickStart += VirtualJoystick_OnJoystickStart;
        virtualJoystick.OnJoystickUpdate += VirtualJoystick_OnJoystickUpdate;
    }

    private void VirtualJoystick_OnJoystickStart(Vector3 arg0)
    {
        interactionController.Unselect();
    }

    private void VirtualJoystick_OnJoystickUpdate(Vector3 direction)
    {
        direction.z = direction.y;
        direction.y = 0;

        var targetPosition = playerReference.Player.transform.position + direction.normalized;
        playerReference.SetTargetPosition(targetPosition);

        cameraPanController.CenterTargetInView(targetPosition);
    }

    private void UseOrthographicCamera()
    {
        // Swap between orthographic and perspective
        if (!mainCamera.orthographic)
        {
            cameraPanController.enabled = true;
            photoVirtualCamera.Priority = 0;
            mainCamera.orthographic = true;  // Isometric / orthographic
        }
    }

    private void UsePerspectiveCamera()
    {
        // Swap between orthographic and perspective
        if (mainCamera.orthographic)
        {
            cameraPanController.enabled = false;
            photoVirtualCamera.Priority = 12;
            mainCamera.orthographic = false; // Perspective
        }
    }
}
