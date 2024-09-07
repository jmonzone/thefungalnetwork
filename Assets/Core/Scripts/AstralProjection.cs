using UnityEngine;

public class AstralProjection : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ProximityAction playerInteraction; 

    // Start is called before the first frame update
    void Start()
    {
        FungalManager.Instance.OnInteractionStarted += OnFungalInteractionStarted;

        playerInteraction.OnUse += ReturnToBody;
    }

    private void ReturnToBody()
    {
        playerAnimator.SetTrigger("returnToBody");

        var movement = playerInteraction.GetComponent<MoveController>();
        PlayerController.Instance.SetMovementController(movement);
    }

    private void OnFungalInteractionStarted(FungalController fungal)
    {
        playerAnimator.SetFloat("randomValue", Random.Range(0f, 1f));
        playerAnimator.SetTrigger("leaveBody");
        PlayerController.Instance.SetMovementController(fungal.Movement);

    }
}
