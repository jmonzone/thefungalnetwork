using UnityEngine;

public class AstralProjection : MonoBehaviour
{
    [SerializeField] private AvatarAnimation avatar;
    [SerializeField] private Controllable avatarControllable;
    [SerializeField] private Controller controller;

    private void Awake()
    {
        var groveManager = GetComponent<GroveManager>();
        groveManager.OnFungalInteraction += PossessFungal;
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
        controller.OnRelease += ReleaseFungal;
    }

    private void OnDisable()
    {
        controller.OnRelease -= ReleaseFungal;
    }

    private void PossessFungal(FungalController fungal)
    {
        avatar.PossessFungal(fungal, () =>
        {
            controller.SetController(fungal.Controllable);
            fungal.GetComponentInChildren<Possessable>().OnPossess();
        });
    }

    private void ReleaseFungal()
    {
        var fungal = controller.Movement;
        fungal.StartRandomMovement();
        avatar.PlayReturnToBodyAnimation();
        controller.SetController(avatarControllable);
    }
}
