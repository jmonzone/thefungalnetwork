using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroveManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private PossesionService possesionService;
    [SerializeField] private FungalService fungalService;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalController fungalControllerPrefab;
    [SerializeField] private EggController eggControllerPrefab;

    [Header("Position References")]
    [SerializeField] private Transform rabbitHolePosition;
    [SerializeField] private Collider fungalBounds;

    private InputManager inputManager;
    private IGroveControllable groveControllable;
    private AstralProjection astralProjection;

    public List<FungalController> FungalControllers { get; private set; } = new List<FungalController>();

    public event UnityAction OnPlayerSpawned;
    public event UnityAction<FungalController> OnFungalInteracted;

    private void Start()
    {
        astralProjection = GetComponent<AstralProjection>();
        astralProjection.OnControllerChanged += controller => SetControllable(controller);

        inputManager = GetComponentInChildren<InputManager>();

        inputManager.OnInteractionButtonClicked += () =>
        {
            groveControllable.Interactions.TargetAction.Use();
        };

        if (fungalService.Fungals.Count == 0)
        {
            var randomIndex = Random.Range(0, fungalCollection.Data.Count);
            var randomFungal = fungalCollection.Data[randomIndex];
            SpawnEgg(randomFungal);
        }

        SpawnFungals();
        SpawnPlayer();
    }

    private ProximityAction previousAction;

    private void Update()
    {
        var targetAction = groveControllable.Interactions.TargetAction;

        if (previousAction && previousAction != targetAction) previousAction.SetInRange(false);
        previousAction = targetAction;

        if (targetAction) targetAction.SetInRange(true);
        inputManager.CanInteract(targetAction && targetAction.Interactable);
    }

    private void SpawnPlayer()
    {
        var player = GetComponentInChildren<PlayerController>();

        player.Interaction.OnUse += () => astralProjection.ReturnToTheBody();

        var partner = possesionService.PossessedFungal;
        var targetFungal = FungalControllers.Find(fungal => fungal.Model == partner);
        if (targetFungal)
        {
            targetFungal.transform.position = rabbitHolePosition.position;
            SetControllable(targetFungal);
        }
        else
        {
            player.transform.position = rabbitHolePosition.position;
            SetControllable(player);
        }

        OnPlayerSpawned?.Invoke();
    }

    private void SetControllable(IGroveControllable controllable)
    {
        groveControllable = controllable;
        inputManager.SetControllable(controllable);
        possesionService.SetPossession(controllable is FungalController fungal ? fungal.Model: null);
    }

    private void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        foreach (var fungal in fungalService.Fungals)
        {
            var randomPosition = fungalBounds.GetRandomXZPosition();
            SpawnFungal(fungal, randomPosition);
        }
    }

    private void SpawnFungal(FungalModel fungal, Vector3 spawnPosition)
    {
        var fungalController = Instantiate(fungalControllerPrefab, spawnPosition, Quaternion.identity);
        fungalController.Initialize(fungal, fungalBounds);
        fungalController.transform.forward = Utility.RandomXZVector;
        FungalControllers.Add(fungalController);
        fungalController.OnInteractionStarted += () =>
        {
            astralProjection.PossessFungal(fungalController);
        };
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
        fungalService.AddFungal(fungal);
        SpawnFungal(fungal, egg.transform.position);
    }

}
