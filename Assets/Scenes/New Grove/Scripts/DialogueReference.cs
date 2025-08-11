using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : UIReference
{
    [SerializeField] private List<string> currentDialogue;

    public List<string> CurrentDialogue => currentDialogue;
    
    public event UnityAction<string> OnSpeakerChanged;
    public event UnityAction<List<string>> OnDialogueChanged;

    public void SetSpeaker(string speaker)
    {
        OnSpeakerChanged?.Invoke(speaker);
    }

    public void SetDialogue(List<string> dialogue)
    {
        currentDialogue = dialogue;
        OnDialogueChanged?.Invoke(dialogue);
    }
}
