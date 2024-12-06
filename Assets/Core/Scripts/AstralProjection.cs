using UnityEngine;

public class AstralProjection : MonoBehaviour
{
    [SerializeField] private AvatarAnimation avatar;
    [SerializeField] private Controllable avatarControllable;
    [SerializeField] private Controller controller;

    private void Awake()
    {
        var groveManager = GetComponent<GroveManager>();
        groveManager.OnPlayerSpawned += () =>
        {
            if (controller.Controllable != avatarControllable)
            {
                avatar.PlayLeaveBodyAnimation();
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
        avatarControllable.GetComponent<ProximityAction>().OnUse += () => ReturnToTheBody();
    }

    public void PossessFungal(FungalController fungal)
    {
        if (IsLeavingTheBody)
        {
            avatar.PlayLeaveBodyAnimation();
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

        avatar.PlayReturnToBodyAnimation();

        controller.SetController(avatarControllable);
    }

    private void LeaveFungal()
    {
        var fungal = controller.Movement;
        fungal.StartRandomMovement();
    }

    private bool IsLeavingTheBody => controller.Movement.transform == avatar.transform;
}
