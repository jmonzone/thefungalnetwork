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

    [SerializeField] private float baseSpeed = 0.03f;             // Normal speed between characters
    [SerializeField] private float punctuationPause = 0.2f;       // Extra pause for punctuation

    private void OnEnable()
    {
        dialogue.OnDialogueStart += Dialogue_OnDialogueStart;
    }

    private void Dialogue_OnDialogueStart()
    {
        speakerText.text = dialogue.Unit.Name;
        image.sprite = dialogue.Unit.Sprite;

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
        // Reset button listener so it only affects this dialogue session
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => nextPagePressed = true);

        List<string> pages = dialogue.Unit.Dialogue;

        for (int p = 0; p < pages.Count; p++)
        {
            dialogueText.text = "";
            string fullText = pages[p];
            if (string.IsNullOrEmpty(fullText))
                continue;

            // Typewriter effect with expressive timing
            for (int i = 0; i < fullText.Length; i++)
            {
                dialogueText.text += fullText[i];

                char c = fullText[i];
                float delay = baseSpeed;

                // Extra pause after punctuation
                if (".,!?:;".Contains(c.ToString()))
                    delay += punctuationPause;

                // Slight random variation for organic feel
                delay *= UnityEngine.Random.Range(0.9f, 1.3f);

                yield return new WaitForSeconds(delay);
            }

            // Wait for continue button
            nextPagePressed = false;
            yield return new WaitUntil(() => nextPagePressed);
        }

        // Finished all pages → close dialogue
        dialogue.Close();
    }


}
