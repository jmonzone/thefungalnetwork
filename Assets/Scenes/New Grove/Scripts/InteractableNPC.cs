using UnityEngine;
using UnityEngine.UI;

public class InteractableNPC : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private ViewReference dialogueView;
    [SerializeField] private InteractionButton assistButton;
    [SerializeField] private FrogAssist frogAssist;

    public Transform Transform => transform;

    private void Awake()
    {
        assistButton.OnInteractionClicked += () =>
        {
            if (!frogAssist.AssistActive) frogAssist.StartAssistMode();
            else frogAssist.StopAssistMode();
        };
    }


    public void OnBaseInteraction()
    {
        animator.Play("Jump");
    }
}
