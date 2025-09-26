using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChatUI : DialoguePageUI
{
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button continueButton;

    protected override void Awake()
    {
        base.Awake();
        continueButton.onClick.AddListener(() => OnGlyphDialogueComplete());
    }

    public override void Show()
    {
        base.Show();
        unitImage.sprite = dialogue.Unit.Instance.Data.Sprite;
        unitNameText.text = dialogue.Unit.Instance.Data.Name;
        dialogueText.text = dialogue.Dialogue.Text;
    }

    private void OnGlyphDialogueComplete()
    {
        var targetDialogue = dialogue.Dialogue;

        if (targetDialogue.Responses.Count >= 2)
        {
            var response = targetDialogue.Responses[0];
            dialogue.RespondToChat(response);
            if (response.Next != null) Show();
            else InvokeClose();
        }
        else if (targetDialogue.Next != null)
        {
            dialogue.ContinueDialogue();
            Show();
        }
        else
        {
            InvokeClose();
        }
    }

    //private IEnumerator ContinueRoutine()
    //{
    //    yield return new WaitForSeconds(1f);
    //    dialogue.ContinueDialogue();
    //    Show();
    //}

    //private IEnumerator CloseRoutine()
    //{
    //    yield return new WaitForSeconds(2f);
    //    InvokeClose();
    //}
}
