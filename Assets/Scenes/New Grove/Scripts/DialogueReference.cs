using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : UIReference
{
    [SerializeField] private Unit unit;

    public Unit Unit => unit;

    public event UnityAction OnDialogueStart;

    public void ShowDialogue(Unit unit)
    {
        this.unit = unit;
        OnDialogueStart?.Invoke();
    }
}
