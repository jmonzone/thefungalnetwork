using UnityEngine;

public class FungalRace : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;


    private void OnEnable()
    {
        controller.OnInitialize += Controller_OnInitialize;
    }

    private void OnDisable()
    {
        controller.OnInitialize -= Controller_OnInitialize;
    }

    private void Controller_OnInitialize()
    {
        cameraController.Target = controller.Movement.transform;
        controller.Movement.SetSpeed(1f);
        GoToEndPosition();
    }

    private void GoToStartPosition()
    {
        controller.Movement.SetPosition(startPosition.position, GoToEndPosition);
    }

    private void GoToEndPosition()
    {
        controller.Movement.SetPosition(endPosition.position, GoToStartPosition);
    }
}
