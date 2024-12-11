using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//todo: use InitalController script to handle player managerment
//todo: make separate fungal management script
public class GroveManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private Possession possesionService;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;
    [SerializeField] private EggController eggControllerPrefab;
    [SerializeField] private Controllable avatar;

    [Header("Position References")]
    [SerializeField] private Transform rabbitHolePosition;
    [SerializeField] private Collider fungalBounds;

    [SerializeField] private PlayerInput inputManager;
    [SerializeField] private Controller controller;

    public List<FungalController> FungalControllers { get; private set; } = new List<FungalController>();

    public event UnityAction OnPlayerSpawned;
    public event UnityAction<FungalController> OnFungalInteraction;

    private void Start()
    {
        if (fungalInventory.Fungals.Count == 0)
        {
            var randomIndex = Random.Range(0, fungalCollection.Data.Count);
            var randomFungal = fungalCollection.Data[randomIndex];
            SpawnEgg(randomFungal);
        }

        SpawnFungals();
        SpawnPlayer();
    }


    //todo: centralize logic with InitialController
    private void SpawnPlayer()
    {
        Debug.Log("spawning player");

        var partner = possesionService.Fungal;
        var targetFungal = FungalControllers.Find(fungal => fungal.Model == partner);
        if (targetFungal)
        {
            Debug.Log("Setting fungal controller");
            targetFungal.transform.position = rabbitHolePosition.position;

            //todo: centralize logic with AstralProjection
            controller.SetController(targetFungal.Controllable);
            avatar.GetComponent<AvatarAnimation>().PresetFungal(targetFungal.transform);
        }
        else
        {
            Debug.Log("Setting player controller");
            avatar.transform.position = rabbitHolePosition.position;
            controller.SetController(avatar);
        }

        OnPlayerSpawned?.Invoke();
    }

    private void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        foreach (var fungal in fungalInventory.Fungals)
        {
            var randomPosition = fungalBounds.GetRandomXZPosition();
            SpawnFungal(fungal, randomPosition);
        }
    }

    private void SpawnEgg(FungalData fungal)
    {
        var randomPosition = (Vector3)Random.insideUnitCircle.normalized * 4;
        randomPosition.z = Mathf.Abs(randomPosition.y);
        randomPosition.y = 1;

        var eggController = Instantiate(eggControllerPrefab, randomPosition, Quaternion.identity);
        eggController.Initialize(fungal);
        eggController.OnHatch += () => OnEggHatched(eggController);
    }

    private void OnEggHatched(EggController egg)
    {
        var fungal = ScriptableObject.CreateInstance<FungalModel>();
        fungal.Initialize(egg.Fungal);
        fungalInventory.AddFungal(fungal);
        SpawnFungal(fungal, egg.transform.position);
    }

    private void SpawnFungal(FungalModel fungal, Vector3 position)
    {
        var fungalController = fungalControllerSpawner.SpawnFungal(fungal, position);
        fungalController.GetComponent<ProximityAction>().OnUse += () => OnFungalInteraction?.Invoke(fungalController);
        FungalControllers.Add(fungalController);
        fungalController.Controllable.Movement.SetBounds(fungalBounds);
    }
}
