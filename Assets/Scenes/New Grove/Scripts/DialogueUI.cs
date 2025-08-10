using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Button continueButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(dialogue.CloseDialogue);
    }

    private void OnEnable()
    {
        dialogue.OnSpeakerChanged += Reference_OnSpeakerChanged;
        dialogue.OnDialogueChanged += Reference_OnDialogueAssigned;
    }

    private void OnDisable()
    {
        dialogue.OnSpeakerChanged -= Reference_OnSpeakerChanged;
        dialogue.OnDialogueChanged -= Reference_OnDialogueAssigned;
    }

    private void Reference_OnSpeakerChanged(string speaker)
    {
        speakerText.text = speaker;
    }

    private void Reference_OnDialogueAssigned(string dialogue)
    {
        dialogueText.text = dialogue;
    }
}
