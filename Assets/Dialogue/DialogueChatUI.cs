using System.Collections;
using System.Linq;
using UnityEngine;

public class DialogueChatUI : DialoguePageUI
{
    [SerializeField] private ResponseUI responseUI;
    [SerializeField] private TypewriterEffect chatTypewriter;
    [SerializeField] private GlyphCollection glyphCollection;

    private GlyphUI glyphUI;

    protected override void Awake()
    {
        base.Awake();
        glyphUI = GetComponent<GlyphUI>();
        glyphUI.OnGlyphMatched += OnGlyphMatched;
    }

    private void OnGlyphMatched()
    {
        responseUI.Show();
        glyphUI.HideGlyphUI();

        var dialogueData = dialogue.Dialogue;
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

    }

    public override void Show()
    {
        base.Show();

        responseUI.Hide();

        var dialogueData = dialogue.Dialogue;

        var availableGlyphs = glyphCollection.Glyphs.Where(glyph => glyph.Tier > 1 && glyph.Element.HasFlag(dialogue.Unit.Instance.Element)).ToList();
        var randomGlyph = availableGlyphs[Random.Range(0, availableGlyphs.Count)];
        glyphUI.StartGlyphDialogue(randomGlyph);

        StartCoroutine(chatTypewriter.TypeRoutine(dialogueData.Text, () =>
        {
            glyphUI.BlockDialogueWithGlyph();
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
