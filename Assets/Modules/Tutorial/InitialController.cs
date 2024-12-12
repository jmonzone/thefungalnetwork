using UnityEngine;
using UnityEngine.Events;

//todo: name something more like PlayerHandler or PlayerController
public class InitialController : MonoBehaviour
{
    //todo: reorder fields
    [SerializeField] private Possession possession;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;
    [SerializeField] private FungalModel initalFungal;
    [SerializeField] private Controllable avatarPrefab;
    [SerializeField] private Controller controller;

    public FungalModel InitalFungal => initalFungal;

    private Controllable avatar;

    public event UnityAction OnControllerInitialized;

    private void Awake()
    {
        fungalControllerSpawner.OnFungalSpawned += fungal =>
        {
            fungal.GetComponent<ProximityAction>().OnUse += () => PossessFungal(fungal);
        };
    }

    private void Start()
    {
        SpawnPlayer(transform.position);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ReleaseFungal();
        }
    }

    private void OnEnable()
    {
        controller.OnReleaseStart += ReleaseFungal;
    }

    private void OnDisable()
    {
        controller.OnReleaseStart -= ReleaseFungal;
    }

    private void SpawnPlayer(Vector3 spawnPosition)
    {
        Debug.Log("spawning player");

        var partner = possession.Fungal;
        initalFungal = fungalInventory.Fungals.Find(fungal => fungal == partner);
        if (initalFungal)
        {
            var fungalController = fungalControllerSpawner.SpawnFungal(initalFungal, spawnPosition);
            Debug.Log("Setting fungal controller");
            controller.SetController(fungalController.Controllable);
            controller.InitalizePosessable(fungalController.GetComponent<Possessable>());
        }
        else
        {
            Debug.Log("Setting player controller");
            avatar = Instantiate(avatarPrefab, spawnPosition, Quaternion.identity);
            controller.SetController(avatar);
        }

        OnControllerInitialized?.Invoke();
    }

    private void PossessFungal(FungalController fungal)
    {
        fungal.Controllable.Movement.Stop();
        controller.StartPossession(fungal.GetComponent<Possessable>());
    }

    private void ReleaseFungal()
    {
        var fungal = controller.Movement;
        fungal.StartRandomMovement();
        avatar.GetComponent<AvatarAnimation>().StartReleaseAnimation();

        //todo: provide controller with avatar so it can handle
        controller.SetController(avatar);
    }
}
