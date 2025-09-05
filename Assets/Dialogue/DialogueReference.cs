using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : UIReference
{
    [SerializeField] private bool isActive;
    [SerializeField] private UnitController unit;
    [SerializeField] private List<Dialogue> dialogue;

    public bool IsActive => isActive;
    public UnitController Unit => unit;
    public List<Dialogue> Dialogue => dialogue;

    public event UnityAction OnDialogueStart;
    public event UnityAction OnDialogueComplete;

    public void StartDialogue(UnitController unit, List<Dialogue> dialogue)
    {
        isActive = true;
        this.unit = unit;
        this.dialogue = dialogue;
        OnDialogueStart?.Invoke();
    }

    public void CompleteDialogue()
    {
        isActive = false;
        OnDialogueComplete?.Invoke();
    } 
}
