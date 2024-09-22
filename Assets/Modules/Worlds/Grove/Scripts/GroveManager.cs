using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GroveManager : MonoBehaviour
{
    private FungalManager fungalManager;

    public event UnityAction OnPlayerSpawned;

    private void Awake()
    {
        fungalManager = GetComponentInChildren<FungalManager>();
    }

    private void Start()
    {
        fungalManager.SpawnFungals();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var inputManager = GetComponentInChildren<InputManager>();
        var player = GetComponentInChildren<PlayerController>();

        var partner = GameManager.Instance.GetPartner();
        var targetFungal = fungalManager.FungalControllers.Find(fungal => fungal.Model == partner);
        if (targetFungal)
        {
            inputManager.SetMovementController(targetFungal.Movement);
        }
        else
        {
            inputManager.SetMovementController(player.Movement);
        }

        OnPlayerSpawned?.Invoke();
    }
}
