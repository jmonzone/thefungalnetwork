using UnityEngine;
using UnityEngine.Events;

public class AstralProjection : MonoBehaviour
{
    private PlayerController player;
    private InputManager inputManager;

    public event UnityAction<FungalController> OnFungalPossessed;
    public event UnityAction OnReturnedToBody;

    private void Awake()
    {
        inputManager = GetComponentInChildren<InputManager>();

        player = GetComponentInChildren<PlayerController>();

        var groveManager = GetComponent<GroveManager>();
        groveManager.OnPlayerSpawned += () =>
        {
            if (inputManager.Movement != player.Movement)
            {
                player.PlayLeaveBodyAnimation();
            }
        };
    }

    private void Start()
    {
        var groveManager = GetComponent<GroveManager>();
        groveManager.OnFungalInteracted += PossessFungal;
        player.Interaction.OnUse += ReturnToTheBody;
    }

    private void PossessFungal(FungalController fungal)
    {
        if (IsLeavingTheBody)
        {
            player.PlayLeaveBodyAnimation();
        }
        else
        {
            LeaveFungal();
        }

        inputManager.SetMovementController(fungal.Movement);
        GameManager.Instance.SetPartner(fungal);
    }

    private void ReturnToTheBody()
    {
        LeaveFungal();

        player.PlayReturnToBodyAnimation();

        inputManager.SetMovementController(player.Movement);
        GameManager.Instance.SetPartner(null);
    }

    private void LeaveFungal()
    {
        var fungal = inputManager.Movement;
        fungal.StartRandomMovement();
    }

    private bool IsLeavingTheBody => inputManager.Movement.transform == player.transform;
}
