using UnityEngine;
using UnityEngine.Events;

public class AstralProjection : MonoBehaviour
{
    private PlayerController player;
    private InputManager inputManager;

    public event UnityAction<IGroveControllable> OnControllerChanged;

    private void Awake()
    {
        inputManager = GetComponentInChildren<InputManager>();

        player = GetComponentInChildren<PlayerController>();

        var groveManager = GetComponent<GroveManager>();
        groveManager.OnPlayerSpawned += () =>
        {
            if (inputManager.Controllable.Movement != player.Movement)
            {
                player.PlayLeaveBodyAnimation();
            }
        };
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

        OnControllerChanged?.Invoke(fungal);
    }

    public void ReturnToTheBody()
    {
        LeaveFungal();

        player.PlayReturnToBodyAnimation();

        OnControllerChanged?.Invoke(player);
    }

    private void LeaveFungal()
    {
        var fungal = inputManager.Controllable.Movement;
        fungal.StartRandomMovement();
    }

    private bool IsLeavingTheBody => inputManager.Controllable.Movement.transform == player.transform;
}
