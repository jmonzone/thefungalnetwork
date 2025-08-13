using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour, IInteractable
{
    private Animator eyeballAnimator;

    [SerializeField] private DialogueReference dialogue;
    [SerializeField] [TextArea] private List<string> initailDialogue;

    Transform IInteractable.Transform => transform;

    private void Awake()
    {
        eyeballAnimator = GetComponentInChildren<Animator>(true);
    }

    private void OnClose()
    {
        eyeballAnimator.gameObject.SetActive(false);
        eyeballAnimator.enabled = false;
    }

    public void OnSelect()
    {
        eyeballAnimator.gameObject.SetActive(true);
        eyeballAnimator.enabled = true;
        dialogue.SetSpeaker("The Tree");
        dialogue.SetDialogue(initailDialogue);
    }
}
