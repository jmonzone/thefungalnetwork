using UnityEngine;

public class PlayerController : MonoBehaviour, IGroveControllable
{
    private Animator animator;
    public ProximityAction Interaction { get; private set; }
    public ProximityInteraction Interactions { get; private set; }
    public MovementController Movement { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        Interaction = GetComponent<ProximityAction>();
        Interactions = GetComponent<ProximityInteraction>();
        Movement = GetComponent<MovementController>();
    }

    public void PlayLeaveBodyAnimation()
    {
        Debug.Log("playing leave body animation");
        animator.SetFloat("randomValue", Random.Range(0f, 1f));
        animator.SetTrigger("leaveBody");
    }

    public void PlayReturnToBodyAnimation()
    {
        var playerAnimator = GetComponentInChildren<Animator>();
        playerAnimator.SetTrigger("returnToBody");

    }

}
