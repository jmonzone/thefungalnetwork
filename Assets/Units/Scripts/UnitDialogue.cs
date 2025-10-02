using UnityEngine;
using UnityEngine.Events;

public class UnitDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    private UnitController unit;
    private Vector3 originalLookPosition;

    public event UnityAction OnComplete;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
    }

    public void StartDialogue()
    {
        dialogue.StartInteraction(unit, unit.Instance.RandomDialogue);

        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookPosition(playerReference.Player.transform.position);
        }

        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        unit.SetLookPosition(originalLookPosition);
        OnComplete?.Invoke();
    }
}
