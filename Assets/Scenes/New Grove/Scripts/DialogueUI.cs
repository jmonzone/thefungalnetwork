using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private float baseSpeed = 0.03f;             // Normal speed between characters
    [SerializeField] private float punctuationPause = 0.2f;       // Extra pause for punctuation

    [SerializeField] private TarotCardUI tarotCard;
    [SerializeField] private PassTheSpore passTheSpore;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseDialogue);
    }

    private void OnEnable()
    {
        dialogue.OnDialogueStart += Dialogue_OnDialogueStart;
    }

    private void Dialogue_OnDialogueStart()
    {
        tarotCard.gameObject.SetActive(false);

        speakerText.text = dialogue.Unit.Data.Name;
        image.sprite = dialogue.Unit.Data.Sprite;

        StopAllCoroutines();
        StartCoroutine(ShowDialogueRoutine());
        dialogue.Show();
    }

    private void OnDisable()
    {
        dialogue.OnDialogueStart -= Dialogue_OnDialogueStart;
    }

    private bool nextPagePressed;

    private IEnumerator ShowDialogueRoutine()
    {
        List<Dialogue> pages = dialogue.Unit.Data.DialogueList;

        for (int p = 0; p < pages.Count; p++)
        {
            dialogueText.text = "";
            Dialogue fullDialogue = pages[p];

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
                default:
                    continueButton.onClick.AddListener(() => nextPagePressed = true);
                    break;
            }

            if (string.IsNullOrEmpty(fullDialogue.Text)) continue;

            // Typewriter effect with expressive timing
            for (int i = 0; i < fullDialogue.Text.Length; i++)
            {
                dialogueText.text += fullDialogue.Text[i];

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

    private void CloseDialogue()
    {
        // Finished all pages → close dialogue
        dialogue.CompleteDialogue();
        dialogue.Close();
    }

}
