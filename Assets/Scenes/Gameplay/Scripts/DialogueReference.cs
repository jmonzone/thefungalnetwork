using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : UIReference
{
    [SerializeField] private UnitController unit;

    public UnitController Unit => unit;

    public event UnityAction OnDialogueStart;
    public event UnityAction OnDialogueComplete;

    public void StartDialogue(UnitController unit)
    {
        this.unit = unit;
        OnDialogueStart?.Invoke();
    }

    public void CompleteDialogue()
    {
        OnDialogueComplete?.Invoke();
    } 
}
