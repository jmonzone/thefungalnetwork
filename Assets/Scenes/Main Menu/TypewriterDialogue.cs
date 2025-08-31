using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterDialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public FadeCanvasGroup continueButton;

    [TextArea(2, 5)]
    public List<string> paragraphs = new List<string>();

    public float typeSpeed = 0.05f;
    public float paragraphPause = 1.5f;

    private Coroutine typeCoroutine;

    private void OnEnable()
    {
        continueButton.gameObject.SetActive(false);
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        dialogueText.text = "";

        foreach (string paragraph in paragraphs)
        {
            string previousText = dialogueText.text;

            for (int i = 0; i <= paragraph.Length; i++)
            {
                dialogueText.text = previousText + paragraph.Substring(0, i);
                yield return new WaitForSeconds(typeSpeed);
            }

            // After full paragraph, add spacing
            dialogueText.text += "\n\n";
            yield return new WaitForSeconds(paragraphPause);
        }

        yield return continueButton.FadeIn();
    }
}
