using UnityEngine;
using UnityEngine.Events;

public class UnitDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    private UnitController unit;
    private Vector3 originalLookPosition;

    public event UnityAction OnDialogueStarted;
    public event UnityAction OnDialogueCompleted;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
    }

    public void OnInteractionStarted()
    {
        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookPosition(playerReference.Player.transform.position);
        }

        dialogueReference.OnDialogueComplete += Dialogue_OnDialogueComplete;

        OnDialogueStarted?.Invoke();
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogueReference.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        unit.SetLookPosition(originalLookPosition);
        OnDialogueCompleted?.Invoke();
    }
}
