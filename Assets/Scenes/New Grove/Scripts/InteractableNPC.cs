using UnityEngine;
using UnityEngine.UI;

public class InteractableNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Animator animator;

    [SerializeField] private ViewReference dialogueView;
    [SerializeField] private InteractionButton talkButton;
    [SerializeField] private InteractionButton assistButton;
    [SerializeField] private FrogAssist frogAssist;

    [SerializeField] private Navigation navigation;

    public Transform Transform => transform;

    private void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            navigation.GoBack();
        });

        talkButton.OnInteractionClicked += () =>
        {
            navigation.Navigate(dialogueView);
        };

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
