using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueReference reference;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Navigation navigation;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            navigation.GoBack();
        });
    }

    private void OnEnable()
    {
        reference.OnSpeakerChanged += Reference_OnSpeakerChanged;
        reference.OnDialogueChanged += Reference_OnDialogueAssigned;
    }

    private void OnDisable()
    {
        reference.OnSpeakerChanged -= Reference_OnSpeakerChanged;
        reference.OnDialogueChanged -= Reference_OnDialogueAssigned;
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
