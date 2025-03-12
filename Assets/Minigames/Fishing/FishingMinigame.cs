using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    //todo: add to separate ocmponent
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private PufferballReference pufferballReference;

    private void OnEnable()
    {
        playerReference.OnPlayerUpdated += PlayerReference_OnPlayerUpdated;
    }

    private void OnDisable()
    {
        playerReference.OnPlayerUpdated -= PlayerReference_OnPlayerUpdated;
    }

    private void Start()
    {
        pufferballReference.Initialize();
    }

    private void PlayerReference_OnPlayerUpdated()
    {
        cameraController.Target = playerReference.Movement.transform;
    }
}