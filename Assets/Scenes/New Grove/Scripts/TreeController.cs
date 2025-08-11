using System.Collections.Generic;
using UnityEngine;

public class TreeController : InteractableController
{
    private Animator eyeballAnimator;

    [SerializeField] private DialogueReference dialogue;
    [SerializeField] [TextArea] private List<string> initailDialogue;

    protected override UIReference Reference => dialogue;

    protected override void Awake()
    {
        base.Awake();
        eyeballAnimator = GetComponentInChildren<Animator>(true);
    }

    protected override void OnClose()
    {
        base.OnClose();
        eyeballAnimator.gameObject.SetActive(false);
        eyeballAnimator.enabled = false;
    }

    public override void OnSelect()
    {
        base.OnSelect();
        eyeballAnimator.gameObject.SetActive(true);
        eyeballAnimator.enabled = true;
        dialogue.SetSpeaker("The Tree");
        dialogue.SetDialogue(initailDialogue);
    }
}
