using Cinemachine;
using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private CinemachineVirtualCamera photoVirtualCamera;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float zOffset;
    [SerializeField] private float yOffset;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        photoReference.OnPhotoStart += PhotoReference_OnPhotoStart;
        photoReference.OnPhotoExit += UseOrthographicCamera;
        partyReference.OnPartyComplete += UseOrthographicCamera;
    }

    private void OnDisable()
    {
        photoReference.OnPhotoStart -= PhotoReference_OnPhotoStart;
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



    private void PhotoReference_OnPhotoStart()
    {
        //photoVirtualCamera.Follow = playerReference.Player.transform;
        photoVirtualCamera.LookAt = photoReference.LookTarget;
        UsePerspectiveCamera();
    }

    private void UsePerspectiveCamera()
    {
        //photoController.target = photoReference.LookTarget;

        // Swap between orthographic and perspective
        if (mainCamera.orthographic)
        {
            cameraPanController.enabled = false;
            photoVirtualCamera.Priority = 12;
            mainCamera.orthographic = false; // Perspective
        }
    }

    private void Update()
    {
        if (!photoReference.IsActive) return;

        Vector3 playerPos = playerReference.Player.transform.position;
        Vector3 guestPos = photoReference.LookTarget.position;

        // Midpoint between player and guest
        Vector3 direction = (guestPos - playerPos).normalized;

        Vector3 cameraPos = playerPos + direction * zOffset + Vector3.up * yOffset; // 1 unit back

        photoVirtualCamera.transform.position = cameraPos;

    }
}