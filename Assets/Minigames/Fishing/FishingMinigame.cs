using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    //todo: add to separate ocmponent
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private Pufferfish pufferfish;
    [SerializeField] private PufferballReference pufferballReference;

    private void Awake()
    {
        playerReference.OnPlayerUpdated += PlayerReference_OnPlayerUpdated;
    }

    private void Start()
    {
        pufferballReference.SetPufferfish(pufferfish);
    }

    private void PlayerReference_OnPlayerUpdated()
    {
        cameraController.Target = playerReference.Transform;

    }
}