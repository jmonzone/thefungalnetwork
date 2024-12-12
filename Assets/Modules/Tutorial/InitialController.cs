using System.Collections.Generic;
using UnityEngine;

public class InitialController : MonoBehaviour
{
    [SerializeField] private Possession possession;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;

    [SerializeField] private Controllable avatar;
    [SerializeField] private Controller controller;

    private void Start()
    {
        SpawnPlayer(transform.position);
    }

    //todo: centralize with GroveManager
    public void SpawnPlayer(Vector3 spawnPosition)
    {
        Debug.Log("spawning player");

        var partner = possession.Fungal;
        var targetFungal = fungalInventory.Fungals.Find(fungal => fungal == partner);
        if (targetFungal)
        {
            var fungalController = fungalControllerSpawner.SpawnFungal(targetFungal, spawnPosition);
            Debug.Log("Setting fungal controller");

            //todo: centralize logic with AstralProjection
            controller.SetController(fungalController.Controllable);
            controller.InitalizePosessable(fungalController.GetComponent<Possessable>());
        }
        else
        {
            Debug.Log("Setting player controller");
            avatar.transform.position = spawnPosition;
            controller.SetController(avatar);
        }
    }
}
