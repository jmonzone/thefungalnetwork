using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PassTheSpore : MonoBehaviour
{
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private Transform gameCenter;

    private void Awake()
    {
        EndGame();
    }

    public void StartGame()
    {
        virtualCamera.Priority = 11;

        Debug.Log(unitManager.UnitControllers.Count);

        var units = unitManager.UnitControllers;
        int count = units.Count;

        for (int i = 0; i < count; i++)
        {
            var ai = units[i].GetComponent<UnitAI>();

            // Evenly spaced angle around a circle
            float angle = (i / (float)count) * Mathf.PI * 2f;

            // Direction from center
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = gameCenter.transform.position + direction * 1f;

            // Assign destination + facing direction
            ai.SetDestination(destination, -direction); // face toward center
        }

    }

    private void EndGame()
    {
        virtualCamera.Priority = 10;
    }
}
