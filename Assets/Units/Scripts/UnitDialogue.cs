using UnityEngine;

public class UnitDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    private UnitController Unit;
    private Vector3 originalLook;

    private void Awake()
    {
        Unit = GetComponent<UnitController>();
    }

    public void StartDialogue()
    {
        dialogue.StartInteraction(Unit, Unit.Instance.RandomDialogue);

        if (lookAtTarget)
        {
            originalLook = Unit.LookPosition;
            Unit.SetLookPosition(playerReference.Player.transform.position);
        }

        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        Unit.SetLookPosition(originalLook);
    }
}
