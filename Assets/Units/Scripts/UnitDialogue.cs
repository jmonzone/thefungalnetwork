using UnityEngine;

public class UnitDialogue : UnitBehaviour
{
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private PlayerReference playerReference;

    public override void StartBehaviour()
    {
        dialogue.StartDialogue(Unit);
        Unit.LookAt(playerReference.Player.transform.position);

        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        StopBehaviour();
    }
}
