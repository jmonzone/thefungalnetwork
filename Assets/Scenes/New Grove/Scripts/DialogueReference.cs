using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : ScriptableObject
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dialogueView;

    public event UnityAction<string> OnSpeakerChanged;
    public event UnityAction<string> OnDialogueChanged;

    public event UnityAction OnDialogeClosed;

    public void SetSpeaker(string speaker)
    {
        OnSpeakerChanged?.Invoke(speaker);
    }

    public void SetDialogue(string dialogue)
    {
        OnDialogueChanged?.Invoke(dialogue);
    }

    public void ShowDialogue()
    {
        if (navigation.CurrentView != dialogueView)
        {
            navigation.Navigate(dialogueView);
        }
    }

    public void CloseDialogue()
    {
        if (navigation.CurrentView == dialogueView)
        {
            Debug.Log("Closing Dialogue");
            navigation.GoBack();
            OnDialogeClosed?.Invoke();
        }
    }
}
