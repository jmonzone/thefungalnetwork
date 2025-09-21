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

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override void OnBehaviourStart()
    {
        dialogue.StartInteraction(Unit, Unit.Instance.RandomDialogue);

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
