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
        dialogue.StartDialogue(Unit, Unit.Data.DialogueTree.Dialogue);

        if (Unit is FungalController fungal)
        {
            fungal.Focus();
        }

        if (lookAtTarget) Unit.SetLookPosition(playerReference.Player.transform.position);

        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        StopBehaviour();

        if (Unit is FungalController fungal)
        {
            fungal.Unfocus();
        }
    }
}
