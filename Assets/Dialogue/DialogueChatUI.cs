using System.Collections;
using UnityEngine;

public class DialogueChatUI : DialoguePageUI
{
    [SerializeField] private ResponseUI responseUI;
    [SerializeField] private TypewriterEffect chatTypewriter;

    public override void Show()
    {
        base.Show();

        responseUI.Hide();

        var dialogueData = dialogue.Dialogue;
        StartCoroutine(chatTypewriter.TypeRoutine(dialogueData.Text, () =>
        {
            if (dialogueData.Responses.Count >= 2)
            {
                responseUI.ShowResponses(dialogueData.Responses, response =>
                {
                    dialogue.RespondToChat(response);
                    if (response.Next != null) Show();
                    else InvokeClose();
                });
            }
            else if (dialogueData.Next != null)
            {
                StartCoroutine(ContinueRoutine());
            }
            else
            {
                StartCoroutine(CloseRoutine());
            }
        }));
    }

    private IEnumerator ContinueRoutine()
    {
        yield return new WaitForSeconds(1f);
        dialogue.ContinueDialogue();
        Show();
    }

    private IEnumerator CloseRoutine()
    {
        yield return new WaitForSeconds(2f);
        InvokeClose();
    }
}
