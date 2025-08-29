using Cinemachine;
using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private CinemachineVirtualCamera photoVirtualCamera;
    [SerializeField] private PartyReference partyReference;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        photoReference.OnPhotoStart += UsePerspectiveCamera;
        photoReference.OnPhotoExit += UseOrthographicCamera;
        partyReference.OnPartyComplete += UseOrthographicCamera;
    }

    private void OnDisable()
    {
        photoReference.OnPhotoStart -= UsePerspectiveCamera;
        photoReference.OnPhotoExit -= UseOrthographicCamera;
        partyReference.OnPartyComplete -= UseOrthographicCamera;
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