using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        FungalManager.Instance.OnInteractionStarted += _ => LeaveTheBody();
    }

    private void LeaveTheBody()
    {
        animator.SetFloat("randomValue", Random.Range(0, 1));
        animator.SetTrigger("leaveBody");
    }
}
