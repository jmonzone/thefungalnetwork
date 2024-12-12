using UnityEngine;
using UnityEngine.Events;

//todo: name something more like PlayerHandler or PlayerController
public class InitialController : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private Possession possession;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;
    [SerializeField] private Controllable avatarPrefab;
    [SerializeField] private bool forceSpawnAvatar;

    private Controllable avatar;
    private FungalModel initalFungal;

    public FungalModel InitalFungal => initalFungal;

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
        SpawnPlayer();
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

    private void SpawnPlayer()
    {
        Debug.Log("spawning avatar");
        var partner = possession.Fungal;
        initalFungal = fungalInventory.Fungals.Find(fungal => fungal == partner);

        avatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity);

        if (initalFungal && !forceSpawnAvatar)
        {
            var fungalController = fungalControllerSpawner.SpawnFungal(initalFungal, transform.position);
            Debug.Log("Setting fungal controller");
            controller.SetController(fungalController.Controllable);
            controller.InitalizePosessable(fungalController.GetComponent<Possessable>());

            avatar.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.Log("Setting player controller");
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
        controller.SetController(avatar);
    }
}
