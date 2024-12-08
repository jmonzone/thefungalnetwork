using UnityEngine;

public class AvatarAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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
