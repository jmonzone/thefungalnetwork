using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Button continueButton;

    [SerializeField] private float baseSpeed = 0.03f;             // Normal speed between characters
    [SerializeField] private float punctuationPause = 0.2f;       // Extra pause for punctuation

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

    private void Reference_OnDialogueAssigned(List<string> dialogue)
    {
        StopAllCoroutines();
        StartCoroutine(ShowDialogueRoutine());
    }

    private bool nextPagePressed;

    private IEnumerator ShowDialogueRoutine()
    {
        // Reset button listener so it only affects this dialogue session
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => nextPagePressed = true);

        List<string> pages = dialogue.CurrentDialogue;

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
