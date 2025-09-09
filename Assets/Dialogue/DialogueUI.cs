using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private PassTheSpore passTheSpore;

    [Header("UI References")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI chatText;

    [SerializeField] private Button continueButton;
    [SerializeField] private FadeCanvasGroup continueCanvasGroup;

    [SerializeField] private Button closeButton;
    [SerializeField] private TarotCardUI tarotCard;
    [SerializeField] private FadeCanvasGroup chatPage;
    [SerializeField] private FadeCanvasGroup actionPage;
    [SerializeField] private FadeCanvasGroup actionButtons;

    [SerializeField] private FadeCanvasGroup responseCanvasGroup;
    [SerializeField] private Button responseButton1;
    [SerializeField] private Button responseButton2;
    [SerializeField] private TextMeshProUGUI responseText1;
    [SerializeField] private TextMeshProUGUI responseText2;


    [Header("UI Settings")]
    [SerializeField] private float baseSpeed = 0.03f;             // Normal speed between characters
    [SerializeField] private float punctuationPause = 0.2f;       // Extra pause for punctuation

    private bool nextPagePressed;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseDialogue);
    }

    private void OnEnable()
    {
        dialogue.OnDialogueStart += StartTalk;
        dialogue.OnChatStart += Dialogue_OnChatStart;
        dialogue.OnSpecialDialogueStart += Dialogue_OnChatStart;
        dialogue.OnGiveComplete += Dialogue_OnChatStart;
    }

    private void OnDisable()
    {
        dialogue.OnDialogueStart -= StartTalk;
        dialogue.OnChatStart -= Dialogue_OnChatStart;
        dialogue.OnSpecialDialogueStart -= Dialogue_OnChatStart;
        dialogue.OnGiveComplete -= Dialogue_OnChatStart;
    }

    private void StartTalk()
    {
        tarotCard.gameObject.SetActive(false);

        speakerText.text = dialogue.Unit.Data.Name;
        image.sprite = dialogue.Unit.Data.Sprite;

        StopAllCoroutines();
        StartCoroutine(TalkRoutine());
    }

    private IEnumerator TalkRoutine()
    {
        chatPage.gameObject.SetActive(false);
        actionPage.gameObject.SetActive(true);
        actionButtons.gameObject.SetActive(false);


        dialogueText.text = "";
        var randomIndex = Random.Range(0, dialogue.Unit.Data.Intros.Count);
        var fullDialogue = dialogue.Unit.Data.Intros[randomIndex];

        if (string.IsNullOrEmpty(fullDialogue)) yield break;

        // Typewriter effect with expressive timing
        for (int i = 0; i < fullDialogue.Length; i++)
        {
            dialogueText.text += fullDialogue[i];

            char c = fullDialogue[i];
            float delay = baseSpeed;

            // Extra pause after punctuation
            if (".,!?:;".Contains(c.ToString()))
                delay += punctuationPause;

            // Slight random variation for organic feel
            delay *= Random.Range(0.9f, 1.3f);

            yield return new WaitForSeconds(delay);
        }

        yield return actionButtons.FadeIn();
    }

    private void CloseDialogue()
    {
        // Finished all pages → close dialogue
        dialogue.CompleteDialogue();
    }


    private void Dialogue_OnChatStart()
    {
        actionPage.gameObject.SetActive(false);
        tarotCard.gameObject.SetActive(false);

        speakerText.text = dialogue.Unit.Data.Name;
        image.sprite = dialogue.Unit.Data.Sprite;

        StopAllCoroutines();
        StartCoroutine(chatPage.FadeIn());
        StartCoroutine(ChatRoutine(dialogue.Dialogue[0]));
    }

    private IEnumerator ChatRoutine(Dialogue fullDialogue)
    {
        responseCanvasGroup.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        chatText.text = "";
        dialogue.SetCurrentDialogue(fullDialogue);

        // Typewriter effect with expressive timing
        for (int i = 0; i < fullDialogue.Text.Length; i++)
        {
            chatText.text += fullDialogue.Text[i];

            char c = fullDialogue.Text[i];
            float delay = baseSpeed;

            // Extra pause after punctuation
            if (".,!?:;".Contains(c.ToString()))
                delay += punctuationPause;

            // Slight random variation for organic feel
            delay *= Random.Range(0.9f, 1.3f);

            yield return new WaitForSeconds(delay);
        }

        if (fullDialogue.Responses.Count >= 2)
        {
            var response1 = fullDialogue.Responses[0];
            responseButton1.onClick.RemoveAllListeners();
            responseButton1.onClick.AddListener(() =>
            {
                if (response1.HasNext)
                {
                    dialogue.RespondToChat(response1);
                    StartCoroutine(ChatRoutine(response1.Next));
                }
                else
                {
                    CloseDialogue();
                }
            });

            responseText1.text = response1.Text;

            var response2 = fullDialogue.Responses[1];
            responseButton2.onClick.RemoveAllListeners();
            responseButton2.onClick.AddListener(() =>
            {
                if (response2.HasNext)
                {
                    dialogue.RespondToChat(response2);
                    StartCoroutine(ChatRoutine(response2.Next));
                }
                else
                {
                    CloseDialogue();
                }
            });
            responseText2.text = response2.Text;

            yield return responseCanvasGroup.FadeIn();

        }
        else
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                CloseDialogue();
            });
            yield return continueCanvasGroup.FadeIn();
        }
    }


}
