using System.Collections;
using Cinemachine;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;

    private void OnEnable()
    {
        playerReference.OnPlayerUpdated += PlayerReference_OnPlayerUpdated;
    }

    private void OnDisable()
    {
        playerReference.OnPlayerUpdated -= PlayerReference_OnPlayerUpdated;
    }

    private void PlayerReference_OnPlayerUpdated()
    {
        arenaCamera.Priority = 0;
        cameraController.Target = playerReference.Movement.transform;
    }

}