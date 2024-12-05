using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public ProximityAction Interaction { get; private set; }
    public Controllable Controllable { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        Controllable = GetComponent<Controllable>();
        Interaction = GetComponent<ProximityAction>();
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
