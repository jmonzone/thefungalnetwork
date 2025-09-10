using System.Collections;
using System.Linq;
using UnityEngine;

public class UnitDialogue : UnitBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    private Animator animator;
    private AudioSource audioSource;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void OnBehaviourStart()
    {
        dialogue.StartInteraction(Unit, Unit.Instance.Data.ChatDialogue[0]);

        if (lookAtTarget) Unit.SetLookPosition(playerReference.Player.transform.position);

        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
        dialogue.OnDialogueResponse += Dialogue_OnDialogueResponse;
    }

    private void Dialogue_OnDialogueResponse(Response arg0)
    {
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        StopBehaviour();

    }
}
