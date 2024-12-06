using UnityEngine;

public class AstralProjection : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Controller controller;

    private void Awake()
    {
        var groveManager = GetComponent<GroveManager>();
        groveManager.OnPlayerSpawned += () =>
        {
            if (controller.Controllable != player.Controllable)
            {
                player.PlayLeaveBodyAnimation();
            }
        };

        groveManager.OnFungalSpawned += fungal =>
        {
            var action = fungal.GetComponent<ProximityAction>();
            action.OnUse += () => PossessFungal(fungal);
        };
   }

    private void Start()
    {
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
