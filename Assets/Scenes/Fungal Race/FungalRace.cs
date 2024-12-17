using System.Collections.Generic;
using UnityEngine;

public class FungalRace : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform startPosition;
    [SerializeField] private List<Attackable> obstacles;
    [SerializeField] private Transform endPosition;
    [SerializeField] private FungalRaceUI fungalRaceUI;

    [Header("Aura Attributes")]
    [SerializeField] private float lowCooldown;
    [SerializeField] private float mediumCooldown;
    [SerializeField] private float highCooldown;


    private int obstacleIndex = 0;
    private Attackable currentObstacle;
    private float attackTimer = 0;
    private float attackCooldown = 2f;

    private void Awake()
    {
        foreach(var obstacle in obstacles)
        {
            obstacle.OnHealthDepleted += () =>
            {
                currentObstacle = null;
                obstacleIndex++;
                obstacle.gameObject.SetActive(false);
                GoToObstacle();
            };
        }

        fungalRaceUI.OnAuraTypeChanged += () =>
        {
            attackCooldown = fungalRaceUI.AuraType switch
            {
                AuraType.LOW => lowCooldown,
                AuraType.MEDIUM => mediumCooldown,
                AuraType.HIGH => highCooldown,
                _ => mediumCooldown,
            };

            attackTimer = attackCooldown;
        };
    }

    private void Update()
    {
        if (currentObstacle)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                currentObstacle.Damage();
                attackTimer = attackCooldown;
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
        if (obstacleIndex < obstacles.Count)
        {
            var targetObstacle = obstacles[obstacleIndex];
            targetObstacle.gameObject.SetActive(true);
            targetObstacle.Restore();

            controller.Movement.SetTarget(targetObstacle.transform, () =>
            {
                currentObstacle = targetObstacle;
            });
        }
        else
        {
            obstacleIndex = 0;
            GoToEndPosition();
        }
    }

    private void GoToEndPosition()
    {
        controller.Movement.SetPosition(endPosition.position, GoToStartPosition);
    }
}
