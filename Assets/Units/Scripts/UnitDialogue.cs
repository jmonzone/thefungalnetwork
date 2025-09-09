using UnityEngine;

public class UnitDialogue : UnitBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    protected override void OnBehaviourStart()
    {
        dialogue.StartInteraction(Unit, Unit.Data.ChatDialogue[0]);

        if (lookAtTarget) Unit.SetLookPosition(playerReference.Player.transform.position);

        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        StopBehaviour();

    }
}
