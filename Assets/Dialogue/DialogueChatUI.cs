using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
        glyphUI.OnGlyphFused += GlyphUI_OnGlyphFused;
    }

    private void GlyphUI_OnGlyphFused(GlyphController fusedGlyph)
    {
        var matchingGlyph = glyphEmitterUI.GlyphControllers.Find(controller => controller.Glyph == fusedGlyph.Glyph);

        Debug.Log("GlyphUI_OnGlyphFused");

        if (matchingGlyph)
        {
            StartCoroutine(MatchRoutine(fusedGlyph, matchingGlyph));
        }
    }

    private IEnumerator MatchRoutine(GlyphController fusedGlyph, BlockingGlyphController matchingGlyph)
    {
        yield return fusedGlyph.MoveAndSlot(matchingGlyph.RectTransform);
        yield return matchingGlyph.GlowAndRelease();

        yield return new WaitForSeconds(5f);

        yield return matchingGlyph.CleanupTextControllers();

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
            StartCoroutine(ContinueRoutine());
        }
        else
        {
            StartCoroutine(CloseRoutine());
        }
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

        normalTextFade.Hide();
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null) yield break;

        // Wait until the blend is done
        while (brain.IsBlending)
            yield return null;

        yield return new WaitForSeconds(1f);

        // Filter glyphs
        var selectedGlyphs = glyphCollection.Glyphs
            .Where(glyph => glyph.Tier > 1 && glyph.Element.HasFlag(dialogue.Unit.Instance.Element))
            .OrderBy(g => Random.value)
            .Take(1)
            .ToList();

        // Example
        string dialogueText = dialogue.Dialogue.Text;

        // Split into words using spaces (and remove empty entries)
        List<string> words = dialogueText
            .Split(' ')
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToList();


        glyphEmitterUI.EmitGlyphs(selectedGlyphs, words, dialogue.Unit.transform.position);
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
