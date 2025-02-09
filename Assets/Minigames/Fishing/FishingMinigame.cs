using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    //todo: add to separate ocmponent
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraController cameraController;

    private void Awake()
    {
        playerReference.OnPlayerUpdated += PlayerReference_OnPlayerUpdated;
    }

    private void PlayerReference_OnPlayerUpdated()
    {
        cameraController.Target = playerReference.Transform;
    }
}