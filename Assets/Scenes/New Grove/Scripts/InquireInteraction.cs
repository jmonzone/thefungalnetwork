using UnityEngine;

[CreateAssetMenu]
public class InquireInteraction : Interaction
{
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dialogueView;

    public override void OnInteractionStart(Interactable interactable)
    {
        dialogueReference.SetSpeaker(interactable.Id);
        //dialogueReference.SetDialogue(interactable.Dialogue);
        navigation.Navigate(dialogueView);
    }
}
