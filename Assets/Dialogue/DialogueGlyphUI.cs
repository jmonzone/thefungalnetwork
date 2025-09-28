using System.Collections;
using Cinemachine;
using UnityEngine;

public class DialogueGlyphUI : DialoguePageUI
{
    [SerializeField] private GlyphUI glyphUI;

    protected override void Awake()
    {
        base.Awake();
        glyphUI.OnGlyphDialogueComplete += OnGlyphDialogueComplete;
    }

    public override IEnumerator Show()
    {
        yield return base.Show();

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null) yield break;

        // Wait until the blend is done
        while (brain.IsBlending)
            yield return null;

        yield return new WaitForSeconds(1f);

        glyphUI.StartGlyphDialogue();
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
            StartCoroutine(ContinueRoutine());
        }
        else
        {
            StartCoroutine(CloseRoutine());
        }
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
