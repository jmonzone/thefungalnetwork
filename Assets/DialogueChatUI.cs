using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueChatUI : DialoguePageUI
{
    [SerializeField] private DialogueReference dialogue;
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
                }, InvokeClose);
            }
            else
            {
                responseUI.ShowContinue(InvokeClose);
            }
        }));
    }
}
