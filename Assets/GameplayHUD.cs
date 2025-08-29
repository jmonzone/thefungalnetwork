using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private Button cameraButton;
    [SerializeField] private Button backButton;
    [SerializeField] private CinemachineVirtualCamera gameplayVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera photoVirtualCamera;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference cameraView;

    [SerializeField] private PartyReference partyReference;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (cameraButton) cameraButton.onClick.AddListener(UseOrthographicCamera);
        if (backButton) backButton.onClick.AddListener(UsePerspectiveCamera);
    }

    private void OnEnable()
    {
        //partyReference.OnPartyStarted += UsePerspectiveCamera;
        //partyReference.OnPartyComplete += UseOrthographicCamera;
    }

    private void OnDisable()
    {
        //partyReference.OnPartyStarted -= UsePerspectiveCamera;
        //partyReference.OnPartyComplete -= UseOrthographicCamera;
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
