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

    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;

        cameraButton.onClick.AddListener(() =>
        {
            // Swap between orthographic and perspective
            if (mainCamera.orthographic)
            {
                cameraPanController.enabled = false;
                photoVirtualCamera.Priority = 12;
                navigation.Navigate(cameraView);
                mainCamera.orthographic = false; // Perspective
            }
        });

        backButton.onClick.AddListener(() =>
        {
            // Swap between orthographic and perspective
            if (!mainCamera.orthographic)
            {
                cameraPanController.enabled = true;
                photoVirtualCamera.Priority = 0;
                navigation.GoBack();
                mainCamera.orthographic = true;  // Isometric / orthographic
            }
        });
    }
}
