using UnityEngine;

public class FungalRace : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform obstacle;
    [SerializeField] private Transform endPosition;

    private bool isAttacking = false;
    private float obstacleHealth;

    private void Update()
    {
        if (isAttacking)
        {
            obstacleHealth -= Time.deltaTime;
            if (obstacleHealth <= 0)
            {
                isAttacking = false;
                obstacle.gameObject.SetActive(false);
                GoToEndPosition();
            }
        }
    }

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
        GoToObstacle();
    }

    private void GoToStartPosition()
    {
        controller.Movement.SetPosition(startPosition.position, GoToObstacle);
    }

    private void GoToObstacle()
    {
        obstacle.gameObject.SetActive(true);
        obstacleHealth = 10f;

        controller.Movement.SetTarget(obstacle, () =>
        {
            isAttacking = true;
        });
    }

    private void GoToEndPosition()
    {
        controller.Movement.SetPosition(endPosition.position, GoToStartPosition);
    }
}
