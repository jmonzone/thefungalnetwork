using UnityEngine;
using UnityEngine.Events;

public class AstralProjection : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private Controller controller;

    private void Awake()
    {
        player = GetComponentInChildren<PlayerController>();

        var groveManager = GetComponent<GroveManager>();
        groveManager.OnPlayerSpawned += () =>
        {
            if (controller.Controllable != player.Controllable)
            {
                player.PlayLeaveBodyAnimation();
            }
        };

        player.Interaction.OnUse += () => ReturnToTheBody();
    }

    public void PossessFungal(FungalController fungal)
    {
        if (IsLeavingTheBody)
        {
            player.PlayLeaveBodyAnimation();
        }
        else
        {
            LeaveFungal();
        }

        controller.SetController(fungal.Controllable);
    }

    public void ReturnToTheBody()
    {
        LeaveFungal();

        player.PlayReturnToBodyAnimation();

        controller.SetController(player.Controllable);
    }

    private void LeaveFungal()
    {
        var fungal = controller.Movement;
        fungal.StartRandomMovement();
    }

    private bool IsLeavingTheBody => controller.Movement.transform == player.transform;
}
