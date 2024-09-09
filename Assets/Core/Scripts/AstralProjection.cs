using UnityEngine;

public class AstralProjection : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ProximityAction playerInteraction;

    private void Start()
    {
        FungalManager.Instance.OnInteractionStarted += PossessFungal;
        playerInteraction.OnUse += ReturnToTheBody;
    }

    private void PossessFungal(FungalController fungal)
    {
        if (IsLeavingTheBody)
        {
            LeaveTheBody();
        }
        else
        {
            LeaveFungal();
        }

        PlayerController.Instance.SetMovementController(fungal.Movement);
    }

    private void ReturnToTheBody()
    {
        LeaveFungal();

        playerAnimator.SetTrigger("returnToBody");
        var movement = playerInteraction.GetComponent<MoveController>();
        PlayerController.Instance.SetMovementController(movement);
    }

    private void LeaveTheBody()
    {
        playerAnimator.SetFloat("randomValue", Random.Range(0f, 1f));
        playerAnimator.SetTrigger("leaveBody");
    }

    private void LeaveFungal()
    {
        var fungal = PlayerController.Instance.Movement;
        fungal.StartRandomMovement();
    }

    private bool IsLeavingTheBody => PlayerController.Instance.Movement.transform == playerInteraction.transform;
}
