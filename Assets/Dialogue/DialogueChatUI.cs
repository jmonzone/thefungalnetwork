using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChatUI : DialoguePageUI
{
    [SerializeField] private ResponseUI responseUI;
    [SerializeField] private TypewriterEffect chatTypewriter;
    [SerializeField] private Image glyphImage;

    private GlyphPalleteUI glyphPallete;

    protected override void Awake()
    {
        base.Awake();
        glyphPallete = GetComponent<GlyphPalleteUI>();
        glyphPallete.OnGlyphSelected += GlyphPallete_OnGlyphSelected;
    }

    private void GlyphPallete_OnGlyphSelected()
    {
        glyphImage.enabled = false;
    }

    public override void Show()
    {
        base.Show();

        responseUI.Hide();

        var dialogueData = dialogue.Dialogue;
        glyphImage.enabled = dialogueData.Glyph != DialogueGlyph.NONE;

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
