using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : ScriptableObject
{
    public event UnityAction<string> OnSpeakerChanged;
    public event UnityAction<string> OnDialogueChanged;

    public void SetSpeaker(string speaker)
    {
        OnSpeakerChanged?.Invoke(speaker);
    }

    public void SetDialogue(string dialogue)
    {
        OnDialogueChanged?.Invoke(dialogue);
    }
}
