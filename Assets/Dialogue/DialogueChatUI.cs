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
                    Show();
                });
            }
            else
            {
                StartCoroutine(CloseRoutine());
            }
        }));
    }

    private IEnumerator CloseRoutine()
    {
        yield return new WaitForSeconds(2f);
        InvokeClose();
    }
}
