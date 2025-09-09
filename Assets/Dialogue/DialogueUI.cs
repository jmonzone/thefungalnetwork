using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;

    [Header("UI Components")]
    [SerializeField] private SpeakerUI speakerUI;
    [SerializeField] private TypewriterEffect dialogueTypewriter;
    [SerializeField] private TypewriterEffect chatTypewriter;
    [SerializeField] private ResponseUI responseUI;
    [SerializeField] private FadeCanvasGroup chatPage;
    [SerializeField] private FadeCanvasGroup actionPage;
    [SerializeField] private FadeCanvasGroup actionButtons;
    [SerializeField] private TarotCardUI tarotCard;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseDialogue);
    }

    private void OnEnable()
    {
        dialogue.OnInteractionStart += StartInteraction;
        dialogue.OnDialogueStart += StartDialogue;
        dialogue.OnGiveComplete += StartDialogue;
    }

    private void OnDisable()
    {
        dialogue.OnInteractionStart -= StartInteraction;
        dialogue.OnDialogueStart -= StartDialogue;
        dialogue.OnGiveComplete -= StartDialogue;
    }

    private void StartInteraction()
    {
        speakerUI.SetSpeaker(dialogue.Unit.Data);

        actionPage.gameObject.SetActive(true);
        chatPage.gameObject.SetActive(false);
        actionButtons.gameObject.SetActive(false);

        var intro = dialogue.Unit.Data.Intros[Random.Range(0, dialogue.Unit.Data.Intros.Count)];

        StopAllCoroutines();
        StartCoroutine(dialogueTypewriter.TypeRoutine(intro, () => StartCoroutine(actionButtons.FadeIn())));
    }

    private void StartDialogue()
    {
        speakerUI.SetSpeaker(dialogue.Unit.Data);

        actionPage.gameObject.SetActive(false);
        responseUI.gameObject.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(chatPage.FadeIn());

        ShowDialogue(dialogue.Dialogue);
    }

    private void ShowDialogue(Dialogue dialogueData)
    {
        StartCoroutine(chatTypewriter.TypeRoutine(dialogueData.Text, () =>
        {
            if (dialogueData.Responses.Count >= 2)
            {
                responseUI.ShowResponses(dialogueData.Responses, response =>
                {
                    dialogue.RespondToChat(response);
                    ShowDialogue(response.Next);
                }, CloseDialogue);
            }
            else
            {
                responseUI.ShowContinue(CloseDialogue);
            }
        }));
    }

    private void CloseDialogue()
    {
        dialogue.CompleteDialogue();
    }
}
