using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueTree : ScriptableObject
{
    [SerializeField] private List<DialogueSet> allDefaultDialogue;

    public virtual List<Dialogue> Dialogue => GetRandomDialogue(allDefaultDialogue);

    protected List<Dialogue> GetRandomDialogue(List<DialogueSet> dialogues)
    {
        var randomIndex = Random.Range(0, dialogues.Count);
        return dialogues[randomIndex].Dialogue;
    }
}
