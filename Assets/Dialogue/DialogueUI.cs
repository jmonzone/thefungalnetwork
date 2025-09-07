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
    [SerializeField] private Button closeButton;
    [SerializeField] private TarotCardUI tarotCard;
    [SerializeField] private FadeCanvasGroup chatPage;
    [SerializeField] private FadeCanvasGroup actionPage;
    [SerializeField] private FadeCanvasGroup actionButtons;

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
    }

    private void OnDisable()
    {
        dialogue.OnDialogueStart -= StartTalk;
        dialogue.OnChatStart -= Dialogue_OnChatStart;
        dialogue.OnSpecialDialogueStart -= Dialogue_OnChatStart;
    }

    private void StartTalk()
    {
        tarotCard.gameObject.SetActive(false);

        speakerText.text = dialogue.Unit.Data.Name;
        image.sprite = dialogue.Unit.Data.Sprite;

        StopAllCoroutines();
        StartCoroutine(TalkRoutine());
        dialogue.Show();
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
        dialogue.Close();
    }


    private void Dialogue_OnChatStart()
    {
        tarotCard.gameObject.SetActive(false);

        speakerText.text = dialogue.Unit.Data.Name;
        image.sprite = dialogue.Unit.Data.Sprite;

        StopAllCoroutines();
        StartCoroutine(ChatRoutine());
        dialogue.Show();

    }


    private IEnumerator ChatRoutine()
    {
        actionPage.gameObject.SetActive(false);

        StartCoroutine(chatPage.FadeIn());

        List<Dialogue> pages = dialogue.Dialogue;

        for (int p = 0; p < pages.Count; p++)
        {
            chatText.text = "";
            Dialogue fullDialogue = pages[p];
            dialogue.SetCurrentDialogue(fullDialogue);

            continueButton.onClick.RemoveAllListeners();

            switch (fullDialogue.Action)
            {
                case DialogueAction.SHOW_TAROT:
                    tarotCard.Reset();
                    continueButton.onClick.AddListener(() =>
                    {
                        continueButton.interactable = false;
                        tarotCard.StartFlipCard(() =>
                        {
                            continueButton.interactable = true;
                            nextPagePressed = true;
                        });
                    });

                    tarotCard.gameObject.SetActive(true);
                    break;
                case DialogueAction.PLAY_SPORE:
                    continueButton.onClick.AddListener(() =>
                    {
                        CloseDialogue();
                        passTheSpore.StartGame();
                        StopAllCoroutines();
                    });
                    break;
                case DialogueAction.FOLLOW:
                    var fungalController = dialogue.Unit as FungalController;
                    fungalController.SetTarget(playerReference.Player.transform);
                    continueButton.onClick.AddListener(() => nextPagePressed = true);
                    break;
                default:
                    continueButton.onClick.AddListener(() => nextPagePressed = true);
                    break;
            }

            if (string.IsNullOrEmpty(fullDialogue.Text)) continue;

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

            // Wait for continue button
            nextPagePressed = false;
            yield return new WaitUntil(() => nextPagePressed);
        }

        CloseDialogue();
    }

}
