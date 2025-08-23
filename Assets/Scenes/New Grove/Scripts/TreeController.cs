using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour, IInteractable
{
    private Animator eyeballAnimator;

    [SerializeField] private Unit data;
    [SerializeField] private DialogueReference dialogue;

    Transform IInteractable.Transform => transform;

    private void Awake()
    {
        eyeballAnimator = GetComponentInChildren<Animator>(true);
    }

    public void OnSelect()
    {
        eyeballAnimator.gameObject.SetActive(true);
        eyeballAnimator.enabled = true;
        dialogue.ShowDialogue(data);
    }
}
