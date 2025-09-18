using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueChatUI : DialoguePageUI
{
    [SerializeField] private ResponseUI responseUI;
    [SerializeField] private TypewriterEffect chatTypewriter;
    [SerializeField] private GlyphCollection glyphCollection;
    [SerializeField] private GlyphEmitterUI glyphEmitterUI;

    [SerializeField] private TextMeshProUGUI normalText;
    [SerializeField] private FadeCanvasGroup normalTextFade;
    [SerializeField] private FadeCanvasGroup blurTextFade;

    [SerializeField] private FadeCanvasGroup glyphFade;
    [SerializeField] private FadeCanvasGroup responseFade;

    private GlyphUI glyphUI;

    protected override void Awake()
    {
        base.Awake();
        glyphUI = GetComponent<GlyphUI>();
        glyphUI.OnGlyphMatched += OnGlyphMatched;
    }

    private void OnGlyphMatched()
    {
        var dialogueData = dialogue.Dialogue;
        normalText.text = dialogueData.Text;

        StartCoroutine(ShowNormalTextRoutine());
    }

    private IEnumerator ShowNormalTextRoutine()
    {
        StartCoroutine(blurTextFade.FadeOut(2));
        yield return normalTextFade.FadeIn(2);

        //responseUI.Show();
        glyphUI.HideGlyphUI();

        yield return new WaitForSeconds(3f);

        //glyphFade.gameObject.SetActive(false);
        //StartCoroutine(responseFade.FadeIn(1));

        var dialogueData = dialogue.Dialogue;

        if (dialogueData.Responses.Count >= 2)
        {
            var response = dialogueData.Responses[0];
            dialogue.RespondToChat(response);
            if (response.Next != null) Show();
            else InvokeClose();

            //responseUI.ShowResponses(dialogueData.Responses, response =>
            //{
            //    dialogue.RespondToChat(response);
            //    if (response.Next != null) Show();
            //    else InvokeClose();
            //});
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

        //responseFade.gameObject.SetActive(false);
        //StartCoroutine(glyphFade.FadeIn(1));

        normalTextFade.Hide();

        var dialogueData = dialogue.Dialogue;

        // Filter glyphs
        var availableGlyphs = glyphCollection.Glyphs
            .Where(glyph => glyph.Tier > 1 && glyph.Element.HasFlag(dialogue.Unit.Instance.Element))
            .ToList();

        // Pick 3 unique random glyphs (if available)
        var selectedGlyphs = availableGlyphs
            .OrderBy(g => Random.value)   // shuffle
            .Take(3)                      // take first 3
            .ToList();

        glyphUI.StartGlyphDialogue(selectedGlyphs[0]);

        // Emit only the 3 selected glyphs


        //StartCoroutine(blurTextFade.FadeIn());
        StartCoroutine(chatTypewriter.TypeRoutine(dialogueData.Text, () =>
        {
            glyphEmitterUI.EmitGlyphs(selectedGlyphs, dialogue.Unit.transform.position);

            //glyphUI.BlockDialogueWithGlyph();
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
