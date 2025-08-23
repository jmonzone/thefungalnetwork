using UnityEngine;
using UnityEngine.Events;

public class UnitDialogue : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogue;

    private UnitController unit;

    public event UnityAction OnDialogueStart;
    public event UnityAction OnDialogueComplete;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
        unit.OnSelected += Unit_OnSelected;
    }

    private void Unit_OnSelected()
    {
        dialogue.StartDialogue(unit.Data);
        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
        OnDialogueStart?.Invoke();
    }

    private void Dialogue_OnDialogueComplete()
    {
        dialogue.OnDialogueComplete -= Dialogue_OnDialogueComplete;
        OnDialogueComplete?.Invoke();
    }
}
