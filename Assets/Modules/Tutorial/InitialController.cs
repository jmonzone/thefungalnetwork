using UnityEngine;
using UnityEngine.Events;

//todo: name something more like PlayerHandler or PlayerController
public class InitialController : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private Possession possession;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;
    [SerializeField] private MovementController avatarPrefab;
    [SerializeField] private bool forceSpawnAvatar;

    private MovementController avatar;
    private FungalModel initalFungal;

    public FungalModel InitalFungal => initalFungal;

    public event UnityAction OnControllerInitialized;

    private void Awake()
    {
        fungalControllerSpawner.OnFungalSpawned += fungal =>
        {
            var fungalAction = fungal.GetComponent<ProximityAction>();
            fungalAction.OnUse += () =>  PossessFungal(fungal);
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
            fungalController.GetComponent<AbilityCastView>().enabled = true;

            Debug.Log("Setting fungal controller");
            controller.SetMovement(fungalController.Movement);
            controller.InitalizePosessable(fungalController.GetComponent<Possessable>());

            avatar.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.Log("Setting player controller");
            controller.SetMovement(avatar);

            avatar.GetComponent<AbilityCastView>().enabled = true;
        }

        OnControllerInitialized?.Invoke();
    }

    private void PossessFungal(FungalController fungal)
    {
        fungal.Movement.Stop();

        var possessable = fungal.GetComponent<Possessable>();
        controller.StartPossession(possessable);

        fungal.GetComponent<AbilityCastView>().enabled = true;
        avatar.GetComponent<AbilityCastView>().enabled = false;
    }

    private void ReleaseFungal()
    {
        var fungal = controller.Movement;
        fungal.StartRandomMovement();

        fungal.GetComponent<AbilityCastView>().enabled = false;
        avatar.GetComponent<AbilityCastView>().enabled = true;

        avatar.GetComponent<AvatarAnimation>().StartReleaseAnimation();
        controller.SetMovement(avatar);
    }
}
