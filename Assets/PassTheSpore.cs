using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PassTheSpore : MonoBehaviour
{
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private Transform gameCenter;

    public void StartGame()
    {
        virtualCamera.Priority = 11;

        Debug.Log(unitManager.UnitControllers.Count);

        foreach(var unit in unitManager.UnitControllers)
        {
            var ai = unit.GetComponent<UnitAI>();
            var randomDirection = (Vector3)Random.insideUnitCircle.normalized;
            randomDirection.z = randomDirection.y;
            randomDirection.y = 0;
            ai.SetDestination(gameCenter.transform.position + randomDirection * 2f);
        }
    }

    private void EndGame()
    {
        virtualCamera.Priority = 10;
    }
}
