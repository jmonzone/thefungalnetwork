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
    [SerializeField] private UnitListReference unitReference;

    protected override void Awake()
    {
        base.Awake();
        continueButton.onClick.AddListener(() => ContinueDialogue());
    }

    public override IEnumerator Show()
    {
        unitImage.sprite = dialogue.Unit.Instance.Data.Sprite;
        unitNameText.text = dialogue.Unit.Instance.Data.Name;
        dialogueText.text = dialogue.Dialogue.Text;
        yield return base.Show();
    }

    private void ContinueDialogue()
    {
        var targetDialogue = dialogue.Dialogue;

        //if (targetDialogue.Responses.Count >= 2)
        //{
        //    var response = targetDialogue.Responses[0];
        //    dialogue.RespondToChat(response);
        //    if (response.Next != null) StartCoroutine(Show());
        //    else InvokeClose();
        //}

        if (targetDialogue.Next != null)
        {
            dialogue.ContinueDialogue();
            StartCoroutine(Show());
        }
        else
        {
            InvokeClose();
        }
    }
}
