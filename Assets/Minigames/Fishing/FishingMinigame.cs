using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraController cameraController;
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
        cameraController.Target = playerReference.Movement.transform;
    }
}