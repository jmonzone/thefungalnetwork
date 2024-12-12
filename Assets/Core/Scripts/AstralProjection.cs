using UnityEngine;

public class AstralProjection : MonoBehaviour
{
    [SerializeField] private AvatarAnimation avatar;
    [SerializeField] private Controllable avatarControllable;
    [SerializeField] private Controller controller;
    [SerializeField] private FungalControllerSpawner fungalSpawner;

    private void Awake()
    {
        fungalSpawner.OnFungalSpawned += fungal =>
        {
            fungal.GetComponent<ProximityAction>().OnUse += () => PossessFungal(fungal);
        };
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

    private void PossessFungal(FungalController fungal)
    {
        fungal.Controllable.Movement.Stop();
        controller.StartPossession(fungal.GetComponent<Possessable>());
    }

    private void ReleaseFungal()
    {
        var fungal = controller.Movement;
        fungal.StartRandomMovement();
        avatar.StartReleaseAnimation();

        //todo: provide controller with avatar so it can handle
        controller.SetController(avatarControllable);
    }
}
