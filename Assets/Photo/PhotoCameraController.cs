using Cinemachine;
using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private CinemachineVirtualCamera photoVirtualCamera;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float zOffset;
    [SerializeField] private float yOffset;

    private void OnEnable()
    {
        photoReference.OnPhotoStart += OnPhotoStart;
        photoReference.OnPhotoExit += OnPhotoExit;
        partyReference.OnPartyComplete += OnPhotoExit;
    }

    private void OnDisable()
    {
        photoReference.OnPhotoStart -= OnPhotoStart;
        photoReference.OnPhotoExit -= OnPhotoExit;
        partyReference.OnPartyComplete -= OnPhotoExit;
    }

    private void OnPhotoExit()
    {
        photoVirtualCamera.Priority = 0;
    }

    private void OnPhotoStart()
    {
        photoVirtualCamera.LookAt = photoReference.LookTarget;
        photoVirtualCamera.Priority = 12;
    }

    private void Update()
    {
        if (!photoReference.IsActive) return;

        Vector3 playerPos = playerReference.Player.transform.position;
        Vector3 guestPos = playerPos + Vector3.forward * 5f;
        if (photoReference.LookTarget) guestPos = photoReference.LookTarget.position;

        // Midpoint between player and guest
        Vector3 direction = (guestPos - playerPos).normalized;

        Vector3 cameraPos = playerPos + direction * zOffset + Vector3.up * yOffset; // 1 unit back

        photoVirtualCamera.transform.position = cameraPos;

    }
}